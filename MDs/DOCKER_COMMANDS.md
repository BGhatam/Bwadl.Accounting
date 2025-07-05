# Docker Commands Reference for Bwadl Accounting API

## Building and Running

### Build the Docker image
```bash
# Build just the API image
docker build -t bwadl-accounting-api .

# Build with docker-compose (includes all services)
docker-compose build
```

### Run with Docker (single container)
```bash
# Run the API container alone (requires external database)
docker run -d \
  --name bwadl-api \
  -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=bwadl_accounting;Username=postgres;Password=yourpassword" \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -v $(pwd)/logs:/app/logs \
  bwadl-accounting-api
```

### Run with Docker Compose (full stack)
```bash
# Development - with all dependencies
docker-compose up -d

# Production - using production compose file
docker-compose -f docker-compose.prod.yml up -d

# Build and run in one command
docker-compose up --build -d

# Run with specific environment file
docker-compose --env-file .env.production up -d
```

## Port Mapping Examples

```bash
# Map different host ports to container port 8080
docker run -p 3000:8080 bwadl-api    # Access via localhost:3000
docker run -p 5000:8080 bwadl-api    # Access via localhost:5000
docker run -p 80:8080 bwadl-api      # Access via localhost (port 80)
```

## Volume Examples

### Bind Mounts (map host directories)
```bash
# Mount logs directory
docker run -v $(pwd)/logs:/app/logs bwadl-api

# Mount configuration (read-only)
docker run -v $(pwd)/config:/app/config:ro bwadl-api

# Mount multiple directories
docker run \
  -v $(pwd)/logs:/app/logs \
  -v $(pwd)/data:/app/data \
  -v $(pwd)/config:/app/config:ro \
  bwadl-api
```

### Named Volumes (Docker-managed)
```bash
# Create named volume
docker volume create bwadl-logs

# Use named volume
docker run -v bwadl-logs:/app/logs bwadl-api

# List volumes
docker volume ls

# Inspect volume
docker volume inspect bwadl-logs
```

## Environment Variables

### Setting Environment Variables at Different Levels

#### 1. In Dockerfile (build-time defaults)
```dockerfile
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ConnectionStrings__DefaultConnection=""
```

#### 2. With docker run (runtime override)
```bash
# Single environment variable
docker run -e ASPNETCORE_ENVIRONMENT=Development bwadl-api

# Multiple environment variables
docker run \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ConnectionStrings__DefaultConnection="Host=localhost;Database=test" \
  -e Security__Jwt__SecretKey="your-secret-key" \
  bwadl-api

# From environment file
docker run --env-file .env bwadl-api
```

#### 3. With docker-compose (runtime, in YAML)
```yaml
# In docker-compose.yml
services:
  bwadl-api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=bwadl
    # Or from external file
    env_file:
      - .env
```

#### 4. Environment Variable Precedence (highest to lowest)
1. `docker run -e` (highest priority)
2. `docker-compose environment:` section
3. `docker-compose env_file:`
4. `--env-file` flag
5. Dockerfile `ENV` (lowest priority)

## Complete Example Commands

### Development Setup
```bash
# 1. Copy environment template
cp .env.example .env

# 2. Edit .env with your values
nano .env

# 3. Start all services
docker-compose up -d

# 4. View logs
docker-compose logs -f bwadl-api

# 5. Access the application
open http://localhost:8080/swagger
```

### Production Deployment
```bash
# 1. Create production environment file
cp .env.example .env.production

# 2. Set production values in .env.production
# 3. Deploy with production compose
docker-compose -f docker-compose.prod.yml --env-file .env.production up -d

# 4. Monitor logs
docker-compose -f docker-compose.prod.yml logs -f
```

### Debugging and Maintenance

```bash
# View running containers
docker-compose ps

# View logs of specific service
docker-compose logs -f bwadl-api

# Execute commands inside running container
docker-compose exec bwadl-api bash

# Restart specific service
docker-compose restart bwadl-api

# Stop all services
docker-compose down

# Stop and remove volumes (⚠️ data loss)
docker-compose down -v

# View resource usage
docker stats

# Clean up unused images/containers
docker system prune -a
```

## Health Checks and Monitoring

```bash
# Check container health
docker inspect bwadl-api | grep -A 10 Health

# View health check logs
docker logs bwadl-api 2>&1 | grep health

# Test health endpoint manually
curl http://localhost:8080/health
```

## Troubleshooting

### Common Issues and Solutions

1. **Port already in use**
   ```bash
   # Find what's using the port
   lsof -i :8080
   
   # Use different port
   docker run -p 8081:8080 bwadl-api
   ```

2. **Database connection issues**
   ```bash
   # Check if postgres is running
   docker-compose ps postgres
   
   # Check postgres logs
   docker-compose logs postgres
   
   # Test connection from API container
   docker-compose exec bwadl-api curl postgres:5432
   ```

3. **Volume permission issues**
   ```bash
   # Fix permissions for logs directory
   sudo chown -R $(whoami):$(whoami) ./logs
   chmod 755 ./logs
   ```

4. **Environment variable not working**
   ```bash
   # Debug environment variables in container
   docker-compose exec bwadl-api env | grep ConnectionStrings
   
   # Check if .env file is loaded
   docker-compose config
   ```

## CI/CD Integration

### GitHub Actions with PostgreSQL Services

Your CI/CD pipeline now includes PostgreSQL and Redis services for integration testing:

```yaml
# .github/workflows/ci-cd.yml
services:
  postgres:
    image: postgres:15
    env:
      POSTGRES_DB: bwadl_accounting_test
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: test_password
    options: >-
      --health-cmd pg_isready
      --health-interval 10s
      --health-timeout 5s
      --health-retries 5
    ports:
      - 5432:5432
```

### Running Tests Locally with Docker

```bash
# Start test environment
docker-compose -f docker-compose.yml up -d postgres redis

# Run tests with the same environment as CI/CD
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=bwadl_accounting_test;Username=postgres;Password=test_password"
export ASPNETCORE_ENVIRONMENT="Testing"
dotnet test --configuration Release

# Cleanup
docker-compose down
```

### Integration Test Strategies

The project supports multiple test database strategies:

1. **PostgreSQL Service (CI/CD)**: Uses real PostgreSQL in GitHub Actions
2. **In-Memory Database (Fallback)**: Uses EF Core InMemory provider when database unavailable
3. **Docker Testcontainers**: Can spin up isolated database containers per test

```bash
# Test with real database (requires PostgreSQL running)
dotnet test --configuration Release

# Test with in-memory database (no external dependencies)
export ConnectionStrings__DefaultConnection=""
dotnet test --configuration Release
```
