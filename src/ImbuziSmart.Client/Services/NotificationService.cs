using Microsoft.JSInterop;

namespace ImbuziSmart.Client.Services;

public class NotificationService
{
    private readonly IJSRuntime _js;

    public NotificationService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<bool> RequestPermissionAsync()
    {
        var result = await _js.InvokeAsync<string>("Notification.requestPermission");
        return result == "granted";
    }

    public async Task SendAlertAsync(string title, string body)
    {
        await _js.InvokeVoidAsync("imbuziNotify.send", title, body);
    }

    public async Task<bool> IsPermissionGrantedAsync()
    {
        var permission = await _js.InvokeAsync<string>("imbuziNotify.getPermission");
        return permission == "granted";
    }
}
