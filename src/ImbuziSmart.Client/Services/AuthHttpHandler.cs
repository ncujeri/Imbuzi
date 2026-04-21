using System.Net.Http.Headers;
using ImbuziSmart.Client.Auth;

namespace ImbuziSmart.Client.Services;

/// <summary>
/// DelegatingHandler that attaches the stored JWT Bearer token to every
/// outgoing HTTP request.  Registered as the outer handler for the shared
/// HttpClient so AuthService, SyncService, and any future service that needs
/// API access all send auth headers automatically.
/// </summary>
public class AuthHttpHandler : DelegatingHandler
{
    private readonly ImbuziAuthStateProvider _auth;

    public AuthHttpHandler(ImbuziAuthStateProvider auth) => _auth = auth;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _auth.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
