using Gck.Persistence;
using Gck.Application.Services;
using Microsoft.EntityFrameworkCore;
using Gck.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext with SQL Server
builder.Services.AddDbContext<GckDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add MediatR
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(Gck.Application.Features.Users.Commands.AddUser.AddUserCommand).Assembly));

// Add Application Services
builder.Services.AddScoped<ILoyaltyService, LoyaltyService>();

// Add HttpClient for SMS provider
builder.Services.AddHttpClient();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Gck User Management API", Version = "v1" });
});

// Add CORS - Fixed to include all Blazor client origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        policy => policy
            .WithOrigins(
                "https://localhost:5001", 
                "http://localhost:5000", 
                "https://localhost:7001",
                "http://localhost:5193",   // Added: Blazor WASM client
                "https://localhost:7193")  // Added: HTTPS version
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

// Use centralized exception handling middleware (must be first)
app.UseMiddleware<ExceptionHandlerMiddleware>();

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<GckDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();
    await DbInitializer.InitializeAsync(context, logger);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS must come before UseHttpsRedirection and UseAuthorization
app.UseCors("AllowBlazorClient");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
