using Bwadl.Accounting.Application;
using Bwadl.Accounting.Infrastructure;
using Bwadl.Accounting.Infrastructure.Configuration;
using Bwadl.Accounting.Infrastructure.Data.Seed;
using Bwadl.Accounting.API.Configuration;
using Serilog;
using Prometheus;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/api-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Use Serilog
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Swagger with versioning support
builder.Services.AddSwaggerConfiguration();

// Add API Versioning
builder.Services.AddApiVersioningConfiguration();

// Add Health Check UI (API layer concern)
builder.Services.AddHealthCheckUI(builder.Configuration);

// Add Application and Infrastructure services (Infrastructure includes health checks)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add JWT Authentication and Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCustomAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerConfiguration();
}

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Add middleware
app.UseMiddleware<Bwadl.Accounting.API.Middleware.ExceptionHandlingMiddleware>();
app.UseMiddleware<Bwadl.Accounting.API.Middleware.SecurityHeadersMiddleware>();
app.UseMiddleware<Bwadl.Accounting.Infrastructure.Middleware.ApiKeyMiddleware>();

// Add Prometheus metrics middleware (before authentication)
app.UseMetricServer(); // Exposes /metrics endpoint
app.UseHttpMetrics();  // Collects HTTP metrics

// Add Authentication middleware (after routing)
app.UseAuthentication();

app.UseRouting();

// Add Authorization middleware (after routing, before endpoints)
app.UseAuthorization();

// Enable static files for Health Checks UI (after routing)
app.UseStaticFiles();

// Add Health Check endpoints (comprehensive configuration)
app.UseHealthCheckConfiguration();

app.MapControllers();

// Initialize database
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Testing")
{
    try
    {
        await DatabaseInitializer.InitializeAsync(app.Services);
    }
    catch (Exception ex) when (app.Environment.EnvironmentName == "Testing")
    {
        // In testing environment, log the error but don't fail the app startup
        // This allows tests to run with in-memory database when real database is unavailable
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Database initialization failed in testing environment. Tests may use in-memory database.");
    }
}

try
{
    Log.Information("Starting the application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Make Program accessible for integration tests
public partial class Program { }
