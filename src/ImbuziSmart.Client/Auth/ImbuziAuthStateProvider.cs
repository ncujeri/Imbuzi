using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace ImbuziSmart.Client.Auth;

/// <summary>
/// Custom AuthenticationStateProvider for Blazor WASM.
/// Reads the JWT from localStorage and parses claims without re-validating
/// (server already validated on issue; expiry is checked locally).
/// </summary>
public class ImbuziAuthStateProvider : AuthenticationStateProvider
{
    private const string TokenKey = "imbuzi_auth_token";

    private readonly IJSRuntime _js;

    public ImbuziAuthStateProvider(IJSRuntime js) => _js = js;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return Anonymous();

        var claims = ParseClaims(token);
        if (claims is null)
            return Anonymous();

        // Check expiry locally
        var expClaim = claims.FirstOrDefault(c => c.Type == "exp");
        if (expClaim is not null && long.TryParse(expClaim.Value, out var exp))
        {
            var expiry = DateTimeOffset.FromUnixTimeSeconds(exp);
            if (expiry < DateTimeOffset.UtcNow)
            {
                await ClearTokenAsync();
                return Anonymous();
            }
        }

        var identity  = new ClaimsIdentity(claims, "jwt");
        var principal = new ClaimsPrincipal(identity);
        return new AuthenticationState(principal);
    }

    public async Task<string?> GetTokenAsync()
    {
        try { return await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey); }
        catch { return null; }
    }

    public async Task SetTokenAsync(string token)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task ClearTokenAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static AuthenticationState Anonymous()
        => new(new ClaimsPrincipal(new ClaimsIdentity()));

    /// <summary>
    /// Parses JWT payload claims without signature validation.
    /// Validation is the server's responsibility on issue.
    /// </summary>
    private static List<Claim>? ParseClaims(string jwt)
    {
        try
        {
            var parts   = jwt.Split('.');
            if (parts.Length != 3) return null;

            var payload = parts[1];
            // Pad base64url
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=')
                             .Replace('-', '+').Replace('_', '/');

            var json    = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
            var doc     = JsonDocument.Parse(json);
            var claims  = new List<Claim>();

            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                var type  = prop.Name;
                var value = prop.Value.ValueKind == JsonValueKind.String
                    ? prop.Value.GetString()!
                    : prop.Value.GetRawText();

                // Map standard JWT claim names to ClaimTypes
                var claimType = type switch
                {
                    "sub"   => ClaimTypes.NameIdentifier,
                    "email" => ClaimTypes.Email,
                    "exp"   => "exp",
                    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" => ClaimTypes.Role,
                    "role"  => ClaimTypes.Role,
                    _       => type
                };

                claims.Add(new Claim(claimType, value));
            }

            return claims;
        }
        catch
        {
            return null;
        }
    }
}
