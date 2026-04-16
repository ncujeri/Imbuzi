using System.Net.Http.Json;
using System.Security.Claims;
using ImbuziSmart.Client.Auth;
using ImbuziSmart.Shared.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace ImbuziSmart.Client.Services;

public class AuthService
{
    private readonly HttpClient                  _http;
    private readonly ImbuziAuthStateProvider     _authState;

    public AuthService(HttpClient http, AuthenticationStateProvider authState)
    {
        _http      = http;
        _authState = (ImbuziAuthStateProvider)authState;
    }

    /// <summary>Login and store JWT. Returns null on failure.</summary>
    public async Task<(bool Success, string? Error)> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", request);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadFromJsonAsync<ErrorDto>();
                return (false, err?.Error ?? "Login failed. Check your credentials.");
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result is null) return (false, "Unexpected response from server.");

            await _authState.SetTokenAsync(result.Token);
            return (true, null);
        }
        catch (HttpRequestException)
        {
            return (false, "Cannot reach server. Check your connection.");
        }
    }

    /// <summary>Register a new farm + owner account.</summary>
    public async Task<(bool Success, string? Error)> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", request);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadFromJsonAsync<ErrorsDto>();
                var msg = err?.Errors?.FirstOrDefault() ?? "Registration failed.";
                return (false, msg);
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result is null) return (false, "Unexpected response from server.");

            await _authState.SetTokenAsync(result.Token);
            return (true, null);
        }
        catch (HttpRequestException)
        {
            return (false, "Cannot reach server. Check your connection.");
        }
    }

    public async Task LogoutAsync() => await _authState.ClearTokenAsync();

    public async Task<string?> GetTokenAsync() => await _authState.GetTokenAsync();

    // ── Helpers ───────────────────────────────────────────────────────────────

    private record ErrorDto(string? Error);
    private record ErrorsDto(IEnumerable<string>? Errors);
}
