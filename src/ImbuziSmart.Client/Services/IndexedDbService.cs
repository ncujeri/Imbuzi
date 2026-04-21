using Microsoft.JSInterop;
using System.Text.Json;
using ImbuziSmart.Shared.Sync;

namespace ImbuziSmart.Client.Services;

public class IndexedDbService
{
    private readonly IJSRuntime _js;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    // Stores whose changes should be queued for server sync.
    private static readonly HashSet<string> SyncableStores = new(StringComparer.OrdinalIgnoreCase)
    {
        "animals", "weightRecords", "matingRecords",
        "heatRecords", "medicalLogs", "costEntries"
    };

    public IndexedDbService(IJSRuntime js) => _js = js;

    public async Task<T?> GetAsync<T>(string storeName, Guid id)
    {
        var json = await _js.InvokeAsync<string?>("imbuziDb.getItem", storeName, id.ToString());
        return json is null ? default : JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    /// <summary>Returns the raw JSON for an entity without deserializing — used by SyncService to build payloads.</summary>
    public Task<string?> GetRawAsync(string storeName, Guid id)
        => _js.InvokeAsync<string?>("imbuziDb.getItem", storeName, id.ToString()).AsTask();

    public async Task<List<T>> GetAllAsync<T>(string storeName)
    {
        var json = await _js.InvokeAsync<string>("imbuziDb.getAllItems", storeName);
        return JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? new();
    }

    public async Task<List<T>> GetByIndexAsync<T>(string storeName, string indexName, string key)
    {
        var json = await _js.InvokeAsync<string>("imbuziDb.getByIndex", storeName, indexName, key);
        return JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? new();
    }

    /// <param name="skipQueue">Pass <c>true</c> when writing seed or sync-received data that should NOT re-enter the sync queue.</param>
    public async Task PutAsync<T>(string storeName, T value, bool skipQueue = false)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);
        await _js.InvokeVoidAsync("imbuziDb.putItem", storeName, json);

        if (!skipQueue && SyncableStores.Contains(storeName))
        {
            // Extract the id from the serialised JSON so we don't need a generic constraint.
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("id", out var idProp) &&
                Guid.TryParse(idProp.GetString(), out var entityId))
            {
                await _js.InvokeVoidAsync("imbuziDb.addToSyncQueue", storeName, entityId.ToString(), "upsert");
            }
        }
    }

    /// <param name="skipQueue">Pass <c>true</c> when the delete comes from a sync-receive path.</param>
    public async Task DeleteAsync(string storeName, Guid id, bool skipQueue = false)
    {
        await _js.InvokeVoidAsync("imbuziDb.deleteItem", storeName, id.ToString());

        if (!skipQueue && SyncableStores.Contains(storeName))
        {
            await _js.InvokeVoidAsync("imbuziDb.addToSyncQueue", storeName, id.ToString(), "delete");
        }
    }

    // ── Sync queue helpers ────────────────────────────────────────────────────

    public async Task<List<SyncQueueEntry>> GetSyncQueueAsync()
    {
        var json = await _js.InvokeAsync<string>("imbuziDb.getSyncQueue");
        return JsonSerializer.Deserialize<List<SyncQueueEntry>>(json, JsonOptions) ?? new();
    }

    public async Task<int> CountSyncQueueAsync()
        => await _js.InvokeAsync<int>("imbuziDb.countSyncQueue");

    public async Task RemoveSyncQueueByEntityAsync(string entityType, string entityId)
        => await _js.InvokeVoidAsync("imbuziDb.removeSyncQueueByEntity", entityType, entityId);

    public async Task AddToSyncQueueAsync(string entityType, Guid entityId, string action)
        => await _js.InvokeVoidAsync("imbuziDb.addToSyncQueue", entityType, entityId.ToString(), action);

    public async Task ClearStoreAsync(string storeName)
        => await _js.InvokeVoidAsync("imbuziDb.clearStore", storeName);

    public async Task<bool> IsOnlineAsync()
        => await _js.InvokeAsync<bool>("imbuziDb.isOnline");
}
