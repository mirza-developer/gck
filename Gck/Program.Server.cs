using Gck;
using Blazored.LocalStorage;
using BlazorPro.BlazorSize;
using Gck.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add Blazored LocalStorage for data persistence
builder.Services.AddBlazoredLocalStorage();

// Add BlazorSize for responsive design
builder.Services.AddMediaQueryService();
builder.Services.AddResizeListener();

builder.Services.AddScoped<UserProfileService>();
builder.Services.AddScoped<TournamentService>();
builder.Services.AddSingleton<ApiConfigurationService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Configure HTTP client pointing at the back-end API
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5200/")
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.RunAsync();
