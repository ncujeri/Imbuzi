using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace ImbuziSmart.Client.Services;

/// <summary>
/// Convenience service that extracts tenant/user/role from the
/// current authentication state claims.
/// Falls back to the hardcoded demo tenant if the user is not yet
/// authenticated (offline / dev mode).
/// </summary>
public class CurrentUserService
{
    // The demo tenant seeded in the database — fallback for offline/dev use
    private static readonly Guid DemoTenantId = new("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

    private readonly AuthenticationStateProvider _authStateProvider;

    public CurrentUserService(AuthenticationStateProvider authStateProvider)
        => _authStateProvider = authStateProvider;

    public async Task<Guid> GetTenantIdAsync()
    {
        var state = await _authStateProvider.GetAuthenticationStateAsync();
        var value = state.User.FindFirst("tenant_id")?.Value;
        return Guid.TryParse(value, out var tid) ? tid : DemoTenantId;
    }

    public async Task<Guid?> GetUserIdAsync()
    {
        var state = await _authStateProvider.GetAuthenticationStateAsync();
        var value = state.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(value, out var uid) ? uid : null;
    }

    public async Task<string?> GetRoleAsync()
    {
        var state = await _authStateProvider.GetAuthenticationStateAsync();
        return state.User.FindFirst(ClaimTypes.Role)?.Value;
    }

    public async Task<string?> GetFullNameAsync()
    {
        var state = await _authStateProvider.GetAuthenticationStateAsync();
        return state.User.FindFirst(ClaimTypes.Name)?.Value;
    }

    public async Task<bool> IsInRoleAsync(string role)
    {
        var state = await _authStateProvider.GetAuthenticationStateAsync();
        return state.User.IsInRole(role);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var state = await _authStateProvider.GetAuthenticationStateAsync();
        return state.User.Identity?.IsAuthenticated == true;
    }
}
