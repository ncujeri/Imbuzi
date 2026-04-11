using Microsoft.JSInterop;

namespace ImbuziSmart.Client.Services;

/// <summary>
/// Manages dark / light theme. Reads the saved preference from localStorage via JS
/// and applies it by setting data-theme on &lt;html&gt;.
/// </summary>
public class ThemeService
{
    private readonly IJSRuntime _js;

    public string Current { get; private set; } = "dark";
    public bool IsDark => Current == "dark";

    /// <summary>Raised whenever the theme changes so subscribed components can re-render.</summary>
    public event Action? OnChange;

    public ThemeService(IJSRuntime js) => _js = js;

    /// <summary>Call once from MainLayout.OnAfterRenderAsync to sync with localStorage.</summary>
    public async Task InitAsync()
    {
        try
        {
            Current = await _js.InvokeAsync<string>("imbuziTheme.get");
        }
        catch
        {
            Current = "dark";
        }
        OnChange?.Invoke();
    }

    public async Task ToggleAsync()
    {
        Current = IsDark ? "light" : "dark";
        try
        {
            await _js.InvokeVoidAsync("imbuziTheme.set", Current);
        }
        catch { }
        OnChange?.Invoke();
    }
}
