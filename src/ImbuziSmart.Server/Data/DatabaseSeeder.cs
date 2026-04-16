using Microsoft.AspNetCore.Identity;
using ImbuziSmart.Server.Identity;
using ImbuziSmart.Shared.Auth;
using ImbuziSmart.Shared.Entities;

namespace ImbuziSmart.Server.Data;

/// <summary>
/// Runs at application startup to ensure all roles, the demo tenant,
/// and the super-user account exist in the database.
/// Safe to call on every startup — all operations are idempotent.
/// </summary>
public static class DatabaseSeeder
{
    // Super-user credentials (change password via the app after first login)
    public const string SuperUserEmail    = "admin@imbuzi.app";
    public const string SuperUserPassword = "Admin@2026!";

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope       = services.CreateScope();
        var sp                = scope.ServiceProvider;
        var db                = sp.GetRequiredService<ImbuziDbContext>();
        var roleManager       = sp.GetRequiredService<RoleManager<AppRole>>();
        var userManager       = sp.GetRequiredService<UserManager<AppUser>>();
        var logger            = sp.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder");

        try
        {
            // ── 1. Seed all roles ─────────────────────────────────────────────
            foreach (var roleName in AppRoles.All)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var result = await roleManager.CreateAsync(new AppRole(roleName));
                    if (result.Succeeded)
                        logger.LogInformation("Seeded role: {Role}", roleName);
                    else
                        logger.LogWarning("Failed to seed role {Role}: {Errors}",
                            roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // ── 2. Ensure demo tenant exists ──────────────────────────────────
            var demoTenantId = SeedData.DemoTenantId;
            var tenant       = await db.Tenants.FindAsync(demoTenantId);

            if (tenant is null)
            {
                tenant = new Tenant
                {
                    Id           = demoTenantId,
                    Name         = "Demo Goat Farm",
                    ContactEmail = SuperUserEmail,
                    IsActive     = true,
                    CreatedAt    = DateTime.UtcNow
                };
                db.Tenants.Add(tenant);
                await db.SaveChangesAsync();
                logger.LogInformation("Seeded demo tenant: {TenantId}", demoTenantId);
            }

            // ── 3. Seed super-user (Owner on demo tenant) ─────────────────────
            var superUser = await userManager.FindByEmailAsync(SuperUserEmail);
            if (superUser is null)
            {
                superUser = new AppUser
                {
                    Id        = Guid.NewGuid(),
                    TenantId  = demoTenantId,
                    FirstName = "Super",
                    LastName  = "Admin",
                    Email     = SuperUserEmail,
                    UserName  = SuperUserEmail,
                    IsActive  = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await userManager.CreateAsync(superUser, SuperUserPassword);
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(superUser, AppRoles.Owner);
                    logger.LogInformation(
                        "Seeded super-user: {Email} (role: {Role}, tenant: {TenantId})",
                        SuperUserEmail, AppRoles.Owner, demoTenantId);
                }
                else
                {
                    logger.LogError("Failed to seed super-user: {Errors}",
                        string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}
