using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ImbuziSmart.Server.Data;
using ImbuziSmart.Server.Identity;
using ImbuziSmart.Shared.Auth;
using ImbuziSmart.Shared.Entities;

namespace ImbuziSmart.Server.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser>  _users;
    private readonly RoleManager<AppRole>  _roles;
    private readonly ImbuziDbContext       _db;
    private readonly JwtOptions            _jwt;

    public AuthController(
        UserManager<AppUser>  users,
        RoleManager<AppRole>  roles,
        ImbuziDbContext       db,
        IOptions<JwtOptions>  jwt)
    {
        _users = users;
        _roles = roles;
        _db    = db;
        _jwt   = jwt.Value;
    }

    // ── POST /api/auth/register ───────────────────────────────────────────────
    /// <summary>Creates a new Tenant + Owner account.</summary>
    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Ensure roles exist
        await EnsureRolesAsync();

        // Create Tenant
        var tenant = new Tenant
        {
            Id           = Guid.NewGuid(),
            Name         = req.FarmName.Trim(),
            ContactEmail = req.Email.Trim().ToLowerInvariant(),
            IsActive     = true,
            CreatedAt    = DateTime.UtcNow
        };

        _db.Tenants.Add(tenant);
        await _db.SaveChangesAsync();

        // Create Owner user
        var user = new AppUser
        {
            Id        = Guid.NewGuid(),
            TenantId  = tenant.Id,
            FirstName = req.FirstName.Trim(),
            LastName  = req.LastName.Trim(),
            Email     = req.Email.Trim().ToLowerInvariant(),
            UserName  = req.Email.Trim().ToLowerInvariant(),
            IsActive  = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _users.CreateAsync(user, req.Password);
        if (!createResult.Succeeded)
        {
            // Roll back tenant
            _db.Tenants.Remove(tenant);
            await _db.SaveChangesAsync();
            return BadRequest(new { errors = createResult.Errors.Select(e => e.Description) });
        }

        await _users.AddToRoleAsync(user, AppRoles.Owner);

        var token = GenerateToken(user, AppRoles.Owner, tenant);
        return Ok(BuildResponse(token, user, AppRoles.Owner, tenant));
    }

    // ── POST /api/auth/login ──────────────────────────────────────────────────
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _users.FindByEmailAsync(req.Email);
        if (user is null || !user.IsActive)
            return Unauthorized(new { error = "Invalid credentials." });

        if (!await _users.CheckPasswordAsync(user, req.Password))
            return Unauthorized(new { error = "Invalid credentials." });

        var roles = await _users.GetRolesAsync(user);
        var role  = roles.FirstOrDefault() ?? AppRoles.Viewer;

        var tenant = await _db.Tenants.FindAsync(user.TenantId);
        if (tenant is null || !tenant.IsActive)
            return Unauthorized(new { error = "Tenant account is inactive." });

        var token = GenerateToken(user, role, tenant);
        return Ok(BuildResponse(token, user, role, tenant));
    }

    // ── GET /api/auth/me ──────────────────────────────────────────────────────
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null || !Guid.TryParse(userId, out var uid))
            return Unauthorized();

        var user   = await _users.FindByIdAsync(userId);
        if (user is null) return NotFound();

        var roles  = await _users.GetRolesAsync(user);
        var tenant = await _db.Tenants.FindAsync(user.TenantId);

        return Ok(new
        {
            userId     = user.Id,
            email      = user.Email,
            fullName   = user.FullName,
            role       = roles.FirstOrDefault(),
            tenantId   = user.TenantId,
            tenantName = tenant?.Name
        });
    }

    // ── POST /api/auth/invite ─────────────────────────────────────────────────
    /// <summary>Owner invites a Manager or Viewer to their tenant.</summary>
    [HttpPost("invite")]
    [Authorize(Policy = "OwnerOnly")]
    public async Task<IActionResult> Invite([FromBody] InviteUserRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Only allow Manager or Viewer roles to be invited
        if (req.Role != AppRoles.Manager && req.Role != AppRoles.Viewer)
            return BadRequest(new { error = "Role must be Manager or Viewer." });

        var tenantIdClaim = User.FindFirstValue("tenant_id");
        if (!Guid.TryParse(tenantIdClaim, out var tenantId))
            return Unauthorized();

        await EnsureRolesAsync();

        var user = new AppUser
        {
            Id        = Guid.NewGuid(),
            TenantId  = tenantId,
            FirstName = req.FirstName.Trim(),
            LastName  = req.LastName.Trim(),
            Email     = req.Email.Trim().ToLowerInvariant(),
            UserName  = req.Email.Trim().ToLowerInvariant(),
            IsActive  = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _users.CreateAsync(user, req.Password);
        if (!createResult.Succeeded)
            return BadRequest(new { errors = createResult.Errors.Select(e => e.Description) });

        await _users.AddToRoleAsync(user, req.Role);

        return Ok(new { userId = user.Id, email = user.Email, role = req.Role });
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task EnsureRolesAsync()
    {
        foreach (var roleName in AppRoles.All)
        {
            if (!await _roles.RoleExistsAsync(roleName))
                await _roles.CreateAsync(new AppRole(roleName));
        }
    }

    private string GenerateToken(AppUser user, string role, Tenant tenant)
    {
        var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry      = DateTime.UtcNow.AddDays(_jwt.ExpiryDays);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name,               user.FullName),
            new Claim(ClaimTypes.Role,               role),
            new Claim("tenant_id",                   tenant.Id.ToString()),
            new Claim("tenant_name",                 tenant.Name),
            new Claim("first_name",                  user.FirstName),
            new Claim("last_name",                   user.LastName),
        };

        var token = new JwtSecurityToken(
            issuer:             _jwt.Issuer,
            audience:           _jwt.Audience,
            claims:             claims,
            expires:            expiry,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static LoginResponse BuildResponse(string token, AppUser user, string role, Tenant tenant)
        => new()
        {
            Token      = token,
            Email      = user.Email!,
            FullName   = user.FullName,
            Role       = role,
            TenantId   = tenant.Id,
            TenantName = tenant.Name,
            ExpiresAt  = DateTime.UtcNow.AddDays(30)
        };
}
