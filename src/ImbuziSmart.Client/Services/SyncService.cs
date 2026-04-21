using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.Json;
using ImbuziSmart.Client.Auth;
using ImbuziSmart.Shared.Sync;

namespace ImbuziSmart.Client.Services;

public enum SyncState     { Idle, Syncing, Synced, Failed, Offline }
public enum SyncItemState { Pending, Syncing, Synced, Failed }

public class SyncItemResult
{
    public string        QueueId     { get; set; } = "";
    public string        EntityType  { get; set; } = "";
    public Guid          EntityId    { get; set; }
    public string        DisplayName { get; set; } = "";
    public string        Action      { get; set; } = "";
    public SyncItemState State       { get; set; } = SyncItemState.Pending;
    public string?       Error       { get; set; }
    public string        Timestamp   { get; set; } = "";
}

public class SyncService : IAsyncDisposable
{
    private readonly IndexedDbService       _db;
    private readonly HttpClient             _http;
    private readonly IJSRuntime             _js;
    private readonly ImbuziAuthStateProvider _auth;

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private DotNetObjectReference<SyncService>? _ref;
    private SyncState _state = SyncState.Idle;
    private int       _pendingCount;
    private string?   _errorMessage;
    private bool      _syncInProgress;
    private CancellationTokenSource? _idleResetCts;

    public event Action? OnChanged;

    public SyncState          State        => _state;
    public int                PendingCount => _pendingCount;
    public string?            ErrorMessage => _errorMessage;
    public List<SyncItemResult> SyncItems  { get; private set; } = new();

    public SyncService(
        IndexedDbService        db,
        HttpClient              http,
        IJSRuntime              js,
        ImbuziAuthStateProvider auth)
    {
        _db   = db;
        _http = http;
        _js   = js;
        _auth = auth;
    }

    // ── Init ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Called once by SyncIndicator on first render.
    /// Wires up JS online/offline events and optionally starts an initial pull.
    /// </summary>
    public async Task InitAsync()
    {
        _ref = DotNetObjectReference.Create(this);
        await _js.InvokeVoidAsync("imbuziSync.init", _ref);

        var online = await _db.IsOnlineAsync();
        if (!online)
        {
            SetState(SyncState.Offline);
            return;
        }

        // Push any pending offline changes first
        _pendingCount = await _db.CountSyncQueueAsync();
        if (_pendingCount > 0)
            _ = TriggerSyncAsync();
        else
        {
            // If authenticated and no local data, pull from server (new device / fresh install)
            var token = await _auth.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                var localAnimals = await _db.GetAllAsync<ImbuziSmart.Shared.Entities.Animal>("animals");
                if (localAnimals.Count == 0)
                    _ = PullFromServerAsync();
            }
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

    // ── Push pipeline ─────────────────────────────────────────────────────────

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
                SyncItems.Clear();
                SetState(SyncState.Idle);
                return;
            }

            _pendingCount = queue.Count;
            SetState(SyncState.Syncing);

            // De-duplicate: for the same entity keep only the latest entry.
            var deduped = queue
                .GroupBy(e => (e.EntityType, e.EntityId))
                .Select(g => g.OrderByDescending(e => e.Timestamp).First())
                .ToList();

            var changes = new List<SyncEntry>();
            SyncItems.Clear();

            foreach (var q in deduped)
            {
                if (q.Action == "delete")
                {
                    if (Guid.TryParse(q.EntityId, out var delId))
                    {
                        changes.Add(new SyncEntry
                        {
                            QueueId    = q.Id,
                            EntityType = q.EntityType,
                            EntityId   = delId,
                            Action     = "delete"
                        });
                        SyncItems.Add(new SyncItemResult
                        {
                            QueueId     = q.Id,
                            EntityType  = q.EntityType,
                            EntityId    = delId,
                            DisplayName = delId.ToString()[..8],
                            Action      = "delete",
                            State       = SyncItemState.Pending
                        });
                    }
                }
                else if (Guid.TryParse(q.EntityId, out var upsertId))
                {
                    var payload = await _db.GetRawAsync(q.EntityType, upsertId);
                    if (payload is not null)
                    {
                        changes.Add(new SyncEntry
                        {
                            QueueId    = q.Id,
                            EntityType = q.EntityType,
                            EntityId   = upsertId,
                            Action     = "upsert",
                            Payload    = payload
                        });
                        SyncItems.Add(new SyncItemResult
                        {
                            QueueId     = q.Id,
                            EntityType  = q.EntityType,
                            EntityId    = upsertId,
                            DisplayName = ExtractDisplayName(q.EntityType, payload, upsertId),
                            Action      = q.Action,
                            State       = SyncItemState.Pending
                        });
                    }
                }
            }

            if (changes.Count == 0)
            {
                foreach (var q in queue)
                    await _db.RemoveSyncQueueByEntityAsync(q.EntityType, q.EntityId);
                SyncItems.Clear();
                SetState(SyncState.Idle);
                return;
            }

            // Mark all as syncing
            foreach (var item in SyncItems) item.State = SyncItemState.Syncing;
            OnChanged?.Invoke();

            var request  = new SyncPushRequest { Changes = changes };
            var response = await _http.PostAsJsonAsync("api/sync/push", request);

            if (!response.IsSuccessStatusCode)
            {
                var msg = $"Server returned {(int)response.StatusCode}";
                foreach (var item in SyncItems) { item.State = SyncItemState.Failed; item.Error = msg; }
                _errorMessage = msg;
                SetState(SyncState.Failed);
                return;
            }

            var result = await response.Content.ReadFromJsonAsync<SyncPushResult>();
            if (result is null)
            {
                foreach (var item in SyncItems) { item.State = SyncItemState.Failed; item.Error = "Invalid response"; }
                _errorMessage = "Invalid server response";
                SetState(SyncState.Failed);
                return;
            }

            // Mark per-item results
            var failedQueueIds = result.Errors.ToDictionary(e => e.QueueId, e => e.Error);
            foreach (var item in SyncItems)
            {
                if (failedQueueIds.TryGetValue(item.QueueId, out var err))
                {
                    item.State = SyncItemState.Failed;
                    item.Error = err;
                }
                else
                {
                    item.State = SyncItemState.Synced;
                }
            }

            // Remove successfully applied items from the queue
            foreach (var c in changes)
            {
                if (!failedQueueIds.ContainsKey(c.QueueId))
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

            // Auto-reset to Idle after 6 seconds (enough to read the result panel)
            _idleResetCts = new CancellationTokenSource();
            var cts = _idleResetCts;
            _ = Task.Delay(6000, cts.Token).ContinueWith(t =>
            {
                if (!t.IsCanceled)
                {
                    SyncItems.Clear();
                    SetState(SyncState.Idle);
                }
            }, TaskScheduler.Default);
        }
        catch (Exception ex)
        {
            foreach (var item in SyncItems.Where(i => i.State == SyncItemState.Syncing))
            {
                item.State = SyncItemState.Failed;
                item.Error = ex.Message;
            }
            _errorMessage = ex.Message;
            SetState(SyncState.Failed);
        }
        finally
        {
            _syncInProgress = false;
        }
    }

    // ── Pending-queue snapshot (for Pending Changes page) ────────────────────

    /// <summary>
    /// Returns a de-duplicated, display-name-resolved snapshot of the current
    /// sync queue — used by the Pending Changes page to show what is waiting.
    /// </summary>
    public async Task<List<SyncItemResult>> GetPendingDisplayItemsAsync()
    {
        var queue = await _db.GetSyncQueueAsync();
        if (queue.Count == 0) return new();

        var deduped = queue
            .GroupBy(e => (e.EntityType, e.EntityId))
            .Select(g => g.OrderByDescending(e => e.Timestamp).First())
            .ToList();

        var items = new List<SyncItemResult>();
        foreach (var q in deduped)
        {
            if (!Guid.TryParse(q.EntityId, out var entityId)) continue;

            string displayName;
            if (q.Action == "delete")
            {
                displayName = entityId.ToString()[..8];
            }
            else
            {
                var payload = await _db.GetRawAsync(q.EntityType, entityId);
                displayName = ExtractDisplayName(q.EntityType, payload, entityId);
            }

            items.Add(new SyncItemResult
            {
                QueueId     = q.Id,
                EntityType  = q.EntityType,
                EntityId    = entityId,
                DisplayName = displayName,
                Action      = q.Action,
                Timestamp   = q.Timestamp,
                State       = SyncItemState.Pending
            });
        }
        return items;
    }

    // ── Pull pipeline ─────────────────────────────────────────────────────────

    /// <summary>
    /// Fetches all tenant data from the server and writes it into IndexedDB
    /// (with skipQueue=true so pulled records don't loop back into the queue).
    /// Safe to call when offline — errors are silently ignored.
    /// </summary>
    public async Task PullFromServerAsync()
    {
        var token = await _auth.GetTokenAsync();
        if (string.IsNullOrEmpty(token)) return;          // not authenticated — nothing to pull

        var online = await _db.IsOnlineAsync();
        if (!online) return;

        try
        {
            var response = await _http.GetFromJsonAsync<SyncPullResponse>("api/sync/pull", _jsonOpts);
            if (response is null || !response.Success) return;

            // Only replace local data when the server has records for this tenant.
            // If the server returns empty (data was never pushed), keep local data intact.
            var hasServerData = response.Animals.Count > 0       || response.WeightRecords.Count > 0
                             || response.MatingRecords.Count > 0  || response.HeatRecords.Count > 0
                             || response.MedicalLogs.Count > 0    || response.CostEntries.Count > 0;

            if (hasServerData)
            {
                await _db.ClearStoreAsync("animals");
                await _db.ClearStoreAsync("weightRecords");
                await _db.ClearStoreAsync("matingRecords");
                await _db.ClearStoreAsync("heatRecords");
                await _db.ClearStoreAsync("medicalLogs");
                await _db.ClearStoreAsync("costEntries");
            }

            // Write every entity class into IndexedDB, skipping the sync queue
            foreach (var e in response.Animals)       await _db.PutAsync("animals",       e, skipQueue: true);
            foreach (var e in response.WeightRecords) await _db.PutAsync("weightRecords", e, skipQueue: true);
            foreach (var e in response.MatingRecords) await _db.PutAsync("matingRecords", e, skipQueue: true);
            foreach (var e in response.HeatRecords)   await _db.PutAsync("heatRecords",   e, skipQueue: true);
            foreach (var e in response.MedicalLogs)   await _db.PutAsync("medicalLogs",   e, skipQueue: true);
            foreach (var e in response.CostEntries)   await _db.PutAsync("costEntries",   e, skipQueue: true);
        }
        catch
        {
            // Offline or server unreachable — silently ignore; local data stays intact
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void SetState(SyncState state)
    {
        _state = state;
        OnChanged?.Invoke();
    }

    /// <summary>
    /// Tries to extract a human-readable name from the raw entity JSON.
    /// Falls back to a truncated entity-id suffix.
    /// </summary>
    private static string ExtractDisplayName(string entityType, string? payload, Guid entityId)
    {
        if (payload is not null)
        {
            try
            {
                using var doc = JsonDocument.Parse(payload);
                if (doc.RootElement.TryGetProperty("name", out var nameProp) &&
                    nameProp.GetString() is { Length: > 0 } name)
                    return name;

                if (doc.RootElement.TryGetProperty("tag", out var tagProp) &&
                    tagProp.GetString() is { Length: > 0 } tag)
                    return tag;
            }
            catch { /* malformed JSON */ }
        }

        var shortId = entityId.ToString()[..8];
        return entityType switch
        {
            "weightRecords"  => $"Weight {shortId}",
            "matingRecords"  => $"Mating {shortId}",
            "heatRecords"    => $"Heat {shortId}",
            "medicalLogs"    => $"Medical {shortId}",
            "costEntries"    => $"Cost {shortId}",
            _                => shortId
        };
    }

    public async ValueTask DisposeAsync()
    {
        _idleResetCts?.Cancel();
        _ref?.Dispose();
        await Task.CompletedTask;
    }
}
