using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ImbuziSmart.Client;
using ImbuziSmart.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HTTP client — base address points to the host server
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// Application services (stateless, work fully offline)
builder.Services.AddScoped<WeightCalculatorService>();
builder.Services.AddScoped<GestationTimerService>();
builder.Services.AddScoped<HeatCycleService>();
builder.Services.AddScoped<InbreedingCheckService>();
builder.Services.AddScoped<WithdrawalPeriodService>();
builder.Services.AddScoped<ShadowLedgerService>();

// Infrastructure services (require JS interop)
builder.Services.AddScoped<IndexedDbService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<SyncService>();

// Seed data service
builder.Services.AddScoped<SeedDataService>();

var host = builder.Build();

// Seed demo data into IndexedDB on first launch
var seedService = host.Services.GetRequiredService<SeedDataService>();
await seedService.SeedIfEmptyAsync();

await host.RunAsync();
