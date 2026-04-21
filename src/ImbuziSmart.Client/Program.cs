using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ImbuziSmart.Client;
using ImbuziSmart.Client.Auth;
using ImbuziSmart.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ── Authentication ────────────────────────────────────────────────────────────
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ImbuziAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<ImbuziAuthStateProvider>());
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CurrentUserService>();

// ── HTTP client — attaches JWT to every outgoing API call ─────────────────────
builder.Services.AddTransient<AuthHttpHandler>();
builder.Services.AddScoped(sp =>
{
    var authHandler   = sp.GetRequiredService<AuthHttpHandler>();
    authHandler.InnerHandler = new HttpClientHandler();
    return new HttpClient(authHandler)
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    };
});

// ── Application services (stateless, work fully offline) ──────────────────────
builder.Services.AddScoped<WeightCalculatorService>();
builder.Services.AddScoped<GestationTimerService>();
builder.Services.AddScoped<HeatCycleService>();
builder.Services.AddScoped<InbreedingCheckService>();
builder.Services.AddScoped<WithdrawalPeriodService>();
builder.Services.AddScoped<ShadowLedgerService>();

// ── Infrastructure services (require JS interop) ──────────────────────────────
builder.Services.AddScoped<IndexedDbService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<NotificationBellService>();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<SyncService>();

// ── Seed data service ─────────────────────────────────────────────────────────
builder.Services.AddScoped<SeedDataService>();

var host = builder.Build();

// Seed demo data into IndexedDB only when the user is NOT authenticated.
// Authenticated users will have their real data pulled from the server by
// SyncService.PullFromServerAsync() (triggered from Login.razor or InitAsync).
var seedService = host.Services.GetRequiredService<SeedDataService>();
await seedService.SeedIfEmptyAsync();

await host.RunAsync();
