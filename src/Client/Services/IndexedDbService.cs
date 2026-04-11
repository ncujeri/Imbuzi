using Microsoft.JSInterop;

namespace ImbuziSmart.Client.Services;

public class IndexedDbService
{
    private readonly IJSRuntime _js;

    public IndexedDbService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<T?> GetAsync<T>(string storeName, Guid id)
    {
        return await _js.InvokeAsync<T?>("imbuziDb.getItem", storeName, id.ToString());
    }

    public async Task<List<T>> GetAllAsync<T>(string storeName)
    {
        return await _js.InvokeAsync<List<T>>("imbuziDb.getAllItems", storeName) ?? new List<T>();
    }

    public async Task<List<T>> GetByIndexAsync<T>(string storeName, string indexName, string key)
    {
        return await _js.InvokeAsync<List<T>>("imbuziDb.getByIndex", storeName, indexName, key) ?? new List<T>();
    }

    public async Task PutAsync<T>(string storeName, T value)
    {
        await _js.InvokeVoidAsync("imbuziDb.putItem", storeName, value);
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
