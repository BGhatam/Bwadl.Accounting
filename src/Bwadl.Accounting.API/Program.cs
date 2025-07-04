using Bwadl.Accounting.Application;
using Bwadl.Accounting.Infrastructure;
using Bwadl.Accounting.Infrastructure.Data.Seed;
using Bwadl.Accounting.API.Configuration;
using Serilog;

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

// Add Health Checks
builder.Services.AddHealthCheckConfiguration(builder.Configuration);

// Add Application and Infrastructure services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

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

app.UseRouting();

// Enable static files for Health Checks UI (after routing)
app.UseStaticFiles();

// Add Health Check endpoints (comprehensive configuration)
app.UseHealthCheckConfiguration();

app.MapControllers();

// Initialize database
if (app.Environment.IsDevelopment())
{
    await DatabaseInitializer.InitializeAsync(app.Services);
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
