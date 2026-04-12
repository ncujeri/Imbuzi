using Microsoft.JSInterop;
using System.Net.Http.Json;
using ImbuziSmart.Shared.Sync;

namespace ImbuziSmart.Client.Services;

public enum SyncState { Idle, Syncing, Synced, Failed, Offline }

public class SyncService : IAsyncDisposable
{
    private readonly IndexedDbService _db;
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    private DotNetObjectReference<SyncService>? _ref;
    private SyncState _state = SyncState.Idle;
    private int _pendingCount;
    private string? _errorMessage;
    private bool _syncInProgress;
    private CancellationTokenSource? _idleResetCts;

    public event Action? OnChanged;

    public SyncState State => _state;
    public int PendingCount => _pendingCount;
    public string? ErrorMessage => _errorMessage;

    public SyncService(IndexedDbService db, HttpClient http, IJSRuntime js)
    {
        _db = db;
        _http = http;
        _js = js;
    }

    /// <summary>
    /// Called once by SyncIndicator on first render to wire up JS online/offline events.
    /// </summary>
    public async Task InitAsync()
    {
        _ref = DotNetObjectReference.Create(this);
        await _js.InvokeVoidAsync("imbuziSync.init", _ref);

        // Reflect initial connectivity state
        var online = await _db.IsOnlineAsync();
        if (!online)
        {
            SetState(SyncState.Offline);
        }
        else
        {
            // Trigger sync if there is already queued data (e.g. from a previous offline session)
            _pendingCount = await _db.CountSyncQueueAsync();
            if (_pendingCount > 0)
                _ = TriggerSyncAsync();
        }
    }

    // ── JS-invokable callbacks ────────────────────────────────────────────────

    [JSInvokable]
    public async Task OnOnlineAsync()
    {
        _pendingCount = await _db.CountSyncQueueAsync();
        if (_pendingCount > 0)
            await TriggerSyncAsync();
        else
            SetState(SyncState.Idle);
    }

    [JSInvokable]
    public Task OnOfflineAsync()
    {
        SetState(SyncState.Offline);
        return Task.CompletedTask;
    }

    // ── Sync pipeline ─────────────────────────────────────────────────────────

    public async Task TriggerSyncAsync()
    {
        if (_syncInProgress) return;
        _syncInProgress = true;
        _idleResetCts?.Cancel();

        try
        {
            var queue = await _db.GetSyncQueueAsync();
            if (queue.Count == 0)
            {
                SetState(SyncState.Idle);
                return;
            }

            _pendingCount = queue.Count;
            SetState(SyncState.Syncing);

            // De-duplicate: for the same entity keep only the latest entry.
            // If ANY action is "delete", that wins. Otherwise last "upsert" wins.
            var deduped = queue
                .GroupBy(e => (e.EntityType, e.EntityId))
                .Select(g => g.OrderByDescending(e => e.Timestamp).First())
                .ToList();

            var changes = new List<SyncEntry>();
            foreach (var q in deduped)
            {
                if (q.Action == "delete")
                {
                    if (Guid.TryParse(q.EntityId, out var delId))
                        changes.Add(new SyncEntry { QueueId = q.Id, EntityType = q.EntityType, EntityId = delId, Action = "delete" });
                }
                else if (Guid.TryParse(q.EntityId, out var upsertId))
                {
                    var payload = await _db.GetRawAsync(q.EntityType, upsertId);
                    if (payload is not null)
                        changes.Add(new SyncEntry { QueueId = q.Id, EntityType = q.EntityType, EntityId = upsertId, Action = "upsert", Payload = payload });
                    // If payload is null the entity was deleted after queuing — skip it.
                }
            }

            if (changes.Count == 0)
            {
                // Nothing to push — clear stale queue items and go idle.
                foreach (var q in queue)
                    await _db.RemoveSyncQueueByEntityAsync(q.EntityType, q.EntityId);
                SetState(SyncState.Idle);
                return;
            }

            var request = new SyncPushRequest { Changes = changes };
            var response = await _http.PostAsJsonAsync("api/sync/push", request);

            if (!response.IsSuccessStatusCode)
            {
                _errorMessage = $"Server returned {(int)response.StatusCode}";
                SetState(SyncState.Failed);
                return;
            }

            var result = await response.Content.ReadFromJsonAsync<SyncPushResult>();
            if (result is null)
            {
                _errorMessage = "Invalid server response";
                SetState(SyncState.Failed);
                return;
            }

            // Remove successfully applied items from the queue
            var failedQueueIds = result.Errors.Select(e => e.QueueId).ToHashSet();
            foreach (var c in changes)
            {
                if (!failedQueueIds.Contains(c.QueueId))
                    await _db.RemoveSyncQueueByEntityAsync(c.EntityType, c.EntityId.ToString());
            }

            if (result.Errors.Count > 0)
            {
                _errorMessage = $"{result.Errors.Count} item(s) failed to sync";
                SetState(SyncState.Failed);
                return;
            }

            _errorMessage = null;
            SetState(SyncState.Synced);

            // Auto-reset to Idle after 4 seconds
            _idleResetCts = new CancellationTokenSource();
            var cts = _idleResetCts;
            _ = Task.Delay(4000, cts.Token).ContinueWith(t =>
            {
                if (!t.IsCanceled)
                {
                    SetState(SyncState.Idle);
                }
            }, TaskScheduler.Default);
        }
        catch (Exception ex)
        {
            _errorMessage = ex.Message;
            SetState(SyncState.Failed);
        }
        finally
        {
            _syncInProgress = false;
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void SetState(SyncState state)
    {
        _state = state;
        OnChanged?.Invoke();
    }

    public async ValueTask DisposeAsync()
    {
        _idleResetCts?.Cancel();
        _ref?.Dispose();
        await Task.CompletedTask;
    }
}
