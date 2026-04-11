using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using ImbuziSmart.Client;
using ImbuziSmart.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddFluentUIComponents();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Application services
builder.Services.AddScoped<WeightCalculatorService>();
builder.Services.AddScoped<GestationTimerService>();
builder.Services.AddScoped<HeatCycleService>();
builder.Services.AddScoped<InbreedingCheckService>();
builder.Services.AddScoped<WithdrawalPeriodService>();
builder.Services.AddScoped<ShadowLedgerService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<IndexedDbService>();

await builder.Build().RunAsync();
