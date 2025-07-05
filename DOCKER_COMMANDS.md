# Local Development Guide for Bwadl Accounting API

This project is designed for **local development** with optional Docker services for monitoring.

## Local Development Setup

### Prerequisites
- .NET 8 SDK
- PostgreSQL (local installation or Docker)
- Visual Studio Code or Visual Studio
- Docker Desktop (for monitoring stack only)

### Quick Start

```bash
# 1. Clone and navigate to project
cd /path/to/Bwadl.Accounting

# 2. Set up environment variables
cp .env.example .env
# Edit .env with your local database settings

# 3. Restore dependencies
dotnet restore

# 4. Run database migrations (if you have PostgreSQL running)
dotnet ef database update --project src/Bwadl.Accounting.Infrastructure

# 5. Run the application
dotnet run --project src/Bwadl.Accounting.API

# 6. (Optional) Start monitoring stack
./start-monitoring.sh
```

## Database Options

### Option 1: Local PostgreSQL
```bash
# Install PostgreSQL locally or via Homebrew (macOS)
brew install postgresql
brew services start postgresql

# Create database
createdb bwadl_accounting

# Update .env file
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=bwadl_accounting;Username=postgres;Password=yourpassword
```

### Option 2: PostgreSQL in Docker
```bash
# Run PostgreSQL in Docker
docker run --name postgres-bwadl \
  -e POSTGRES_DB=bwadl_accounting \
  -e POSTGRES_PASSWORD=yourpassword \
  -p 5432:5432 \
  -d postgres:15

# Update .env file
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=bwadl_accounting;Username=postgres;Password=yourpassword
```

### Option 3: In-Memory Database (for testing)
```bash
# Leave connection string empty in .env or remove it
# ConnectionStrings__DefaultConnection=

# The application will automatically use in-memory database
```

## Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Bwadl.Accounting.Tests.Unit
dotnet test tests/Bwadl.Accounting.Tests.Integration

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Monitoring Stack

The monitoring stack runs in Docker containers but monitors your local .NET application:

```bash
# Start monitoring (Prometheus, Grafana, AlertManager)
./start-monitoring.sh

# Stop monitoring
./stop-monitoring.sh

# Access monitoring
open http://localhost:3000  # Grafana (admin/admin)
open http://localhost:9090  # Prometheus
open http://localhost:9093  # AlertManager
```

## Application URLs

When running locally:
- **API**: http://localhost:5281
- **Swagger**: http://localhost:5281/swagger
- **Health Checks**: http://localhost:5281/health
- **Health Checks UI**: http://localhost:5281/healthchecks-ui
- **Metrics**: http://localhost:5281/metrics

## Configuration

### Environment Variables
All configuration can be overridden via environment variables or `.env` file:

```bash
# Application
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:5281

# Database
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=bwadl_accounting;Username=postgres;Password=yourpassword

# Security (Development)
Security__Jwt__ValidateIssuer=false
Security__Jwt__ValidateAudience=false
Security__ApiKeys__RequireHttps=false

# Features (Development)
Features__EnableRateLimiting=false
Features__EnableEmailNotifications=false
Cache__Provider=Memory
MessageBus__Provider=InMemory
```

### appsettings.Development.json
Create this file for development-specific settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=bwadl_accounting;Username=postgres;"
  },
  "Features": {
    "EnableCaching": false,
    "EnableEmailNotifications": false,
    "EnableRateLimiting": false
  }
}
```

## Troubleshooting

### Common Issues

1. **Database connection failed**
   ```bash
   # Check if PostgreSQL is running
   brew services list | grep postgresql
   
   # Check connection
   psql -h localhost -p 5432 -U postgres -d bwadl_accounting
   ```

2. **Port already in use**
   ```bash
   # Find what's using port 5281
   lsof -i :5281
   
   # Kill the process or change port in launchSettings.json
   ```

3. **Monitoring not working**
   ```bash
   # Check if API is exposing metrics
   curl http://localhost:5281/metrics
   
   # Check Docker containers
   docker ps | grep monitoring
   ```

## Development Workflow

1. **Make code changes**
2. **Run locally**: `dotnet run --project src/Bwadl.Accounting.API`
3. **Test changes**: `dotnet test`
4. **Check metrics**: Visit http://localhost:3000 (Grafana)
5. **Debug**: Use VS Code debugger or Visual Studio

## CI/CD Integration

The project includes GitHub Actions that will:
- ✅ Run unit and integration tests
- ✅ Build the application
- ✅ Generate code coverage reports
- ✅ Perform code quality analysis

Tests run with PostgreSQL services in GitHub Actions but fall back to in-memory database when needed.
