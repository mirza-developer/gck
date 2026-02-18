using Gck;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using BlazorPro.BlazorSize;
using Gck.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HTTP client
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add Blazored LocalStorage for data persistence
builder.Services.AddBlazoredLocalStorage();

// Add BlazorSize for responsive design
builder.Services.AddMediaQueryService();
builder.Services.AddResizeListener();

// Add offline and sync services
builder.Services.AddScoped<OfflineStorageService>();
builder.Services.AddScoped<NetworkStatusService>();
builder.Services.AddScoped<SyncService>();

// Add existing services
builder.Services.AddScoped<UserProfileService>();
builder.Services.AddScoped<TournamentService>();
builder.Services.AddSingleton<ApiConfigurationService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

var host = builder.Build();

// Initialize network status service
var networkStatus = host.Services.GetRequiredService<NetworkStatusService>();
await networkStatus.InitializeAsync();

await host.RunAsync();
