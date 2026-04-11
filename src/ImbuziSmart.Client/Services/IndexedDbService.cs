using Microsoft.JSInterop;
using System.Text.Json;

namespace ImbuziSmart.Client.Services;

public class IndexedDbService
{
    private readonly IJSRuntime _js;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public IndexedDbService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<T?> GetAsync<T>(string storeName, Guid id)
    {
        var json = await _js.InvokeAsync<string?>("imbuziDb.getItem", storeName, id.ToString());
        return json is null ? default : JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

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

    public async Task PutAsync<T>(string storeName, T value)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);
        await _js.InvokeVoidAsync("imbuziDb.putItem", storeName, json);
    }

    public async Task DeleteAsync(string storeName, Guid id)
    {
        await _js.InvokeVoidAsync("imbuziDb.deleteItem", storeName, id.ToString());
    }

    public async Task AddToSyncQueueAsync(string entityType, Guid entityId, string action)
    {
        await _js.InvokeVoidAsync("imbuziDb.addToSyncQueue", entityType, entityId.ToString(), action);
    }

    public async Task<bool> IsOnlineAsync()
    {
        return await _js.InvokeAsync<bool>("imbuziDb.isOnline");
    }
}
