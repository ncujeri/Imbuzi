using System.Net.Http.Json;
using System.Text.Json;
using ImbuziSmart.Shared.Entities;
using Microsoft.JSInterop;

namespace ImbuziSmart.Client.Services;

public class NotificationBellService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    private List<AppNotification> _serverNotifications = new();
    private List<AppNotification> _localAlerts = new();
    private readonly HashSet<Guid> _localIds = new();
    private HashSet<string> _dismissedKeys = new();
    private bool _dismissedLoaded;

    private const string StorageKey = "imbuzi_dismissed_alerts";

    public event Action? OnChange;

    public IReadOnlyList<AppNotification> Notifications =>
        _localAlerts.Concat(_serverNotifications).ToList();

    public IReadOnlyList<AppNotification> ServerNotifications => _serverNotifications;

    public int UnreadCount => _localAlerts.Count + _serverNotifications.Count;
    public int ServerCount => _serverNotifications.Count;

    public NotificationBellService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js   = js;
    }

    // Stable key derived from the alert type prefix + entity id so the same
    // real-world condition always maps to the same dismissal key across refreshes.
    private static string AlertKey(AppNotification n)
    {
        var prefix = n.Title.Contains(':') ? n.Title[..n.Title.IndexOf(':')] : n.Title;
        return $"{prefix}:{n.EntityId}";
    }

    private async Task EnsureDismissedLoadedAsync()
    {
        if (_dismissedLoaded) return;
        try
        {
            var json = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            if (json is not null)
                _dismissedKeys = JsonSerializer.Deserialize<HashSet<string>>(json) ?? new();
        }
        catch { }
        _dismissedLoaded = true;
    }

    private async Task SaveDismissedAsync()
    {
        try
        {
            await _js.InvokeVoidAsync("localStorage.setItem", StorageKey,
                JsonSerializer.Serialize(_dismissedKeys));
        }
        catch { }
    }

    public async Task RefreshAsync()
    {
        await EnsureDismissedLoadedAsync();
        try
        {
            var token = await _js.InvokeAsync<string?>("localStorage.getItem", "imbuzi_auth_token");
            if (string.IsNullOrEmpty(token)) return;

            var result = await _http.GetFromJsonAsync<List<AppNotification>>("api/notifications");
            _serverNotifications = result ?? new();
        }
        catch
        {
            _serverNotifications = new();
        }
        OnChange?.Invoke();
    }

    public async Task SetLocalAlertsAsync(List<AppNotification> alerts)
    {
        await EnsureDismissedLoadedAsync();

        foreach (var id in _localAlerts.Select(a => a.Id))
            _localIds.Remove(id);

        // Filter out any alerts the user has already dismissed
        _localAlerts = alerts.Where(a => !_dismissedKeys.Contains(AlertKey(a))).ToList();

        foreach (var a in _localAlerts)
            _localIds.Add(a.Id);

        OnChange?.Invoke();
    }

    public async Task MarkAllReadAsync()
    {
        foreach (var a in _localAlerts)
            _dismissedKeys.Add(AlertKey(a));
        await SaveDismissedAsync();

        _localAlerts.Clear();
        _localIds.Clear();

        try
        {
            await _http.PostAsync("api/notifications/mark-all-read", null);
            _serverNotifications.Clear();
        }
        catch { }

        OnChange?.Invoke();
    }

    public async Task MarkReadAsync(Guid id)
    {
        if (_localIds.Contains(id))
        {
            var alert = _localAlerts.FirstOrDefault(n => n.Id == id);
            if (alert is not null)
            {
                _dismissedKeys.Add(AlertKey(alert));
                await SaveDismissedAsync();
            }
            _localIds.Remove(id);
            _localAlerts.RemoveAll(n => n.Id == id);
            OnChange?.Invoke();
            return;
        }

        try
        {
            await _http.PostAsJsonAsync("api/notifications/mark-read", new List<Guid> { id });
            _serverNotifications.RemoveAll(n => n.Id == id);
            OnChange?.Invoke();
        }
        catch { }
    }
}
