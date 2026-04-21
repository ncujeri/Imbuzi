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

    /// <summary>Invites a new employee (Manager or Viewer) to the current tenant.</summary>
    public async Task<(bool Success, string? Error)> InviteEmployeeAsync(InviteUserRequest request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/invite", request);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadFromJsonAsync<ErrorDto>();
                return (false, err?.Error ?? "Failed to add employee.");
            }
            return (true, null);
        }
        catch (HttpRequestException)
        {
            return (false, "Cannot reach server. Check your connection.");
        }
    }

    /// <summary>Returns all active team members for the current tenant.</summary>
    public async Task<(List<TeamMemberDto>? Members, string? Error)> GetTeamAsync()
    {
        try
        {
            var members = await _http.GetFromJsonAsync<List<TeamMemberDto>>("api/auth/team");
            return (members ?? new(), null);
        }
        catch (HttpRequestException)
        {
            return (null, "Cannot reach server. Check your connection.");
        }
    }

    /// <summary>Deactivates an employee from the current tenant.</summary>
    public async Task<(bool Success, string? Error)> RemoveEmployeeAsync(Guid userId)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/auth/team/{userId}");
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadFromJsonAsync<ErrorDto>();
                return (false, err?.Error ?? "Failed to remove employee.");
            }
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

/// <summary>Team member summary returned from GET /api/auth/team.</summary>
public record TeamMemberDto(
    Guid     UserId,
    string   Email,
    string   FullName,
    string   FirstName,
    string   LastName,
    string   Role,
    DateTime CreatedAt
);
