#!/bin/bash

# Bwadl API Monitoring Stack Startup Script
# This script will start your .NET API, Prometheus, Grafana, and all monitoring components

set -e

echo "🚀 Starting Bwadl API Monitoring Stack..."

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

print_step() {
    echo -e "${BLUE}[STEP]${NC} $1"
}

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    print_error "Docker is not running. Please start Docker Desktop and try again."
    exit 1
fi

print_status "Docker is running ✓"

# Navigate to project directory
PROJECT_DIR="/Users/baqeralghatam/workspace/backend/dotnet/Bwadl.Accounting"
cd "$PROJECT_DIR"

print_step "1. Building .NET Application..."
dotnet build
if [ $? -eq 0 ]; then
    print_status "Build successful ✓"
else
    print_error "Build failed ✗"
    exit 1
fi

print_step "2. Starting Monitoring Stack (Prometheus, Grafana, AlertManager)..."
cd monitoring

# Try modern docker compose first, then fall back to docker-compose
if command -v "docker" >/dev/null 2>&1 && docker compose version >/dev/null 2>&1; then
    print_status "Using modern 'docker compose' command"
    docker compose -f docker-compose.monitoring.yml up -d
elif command -v "docker-compose" >/dev/null 2>&1; then
    print_status "Using legacy 'docker-compose' command"
    docker-compose -f docker-compose.monitoring.yml up -d
else
    print_error "Neither 'docker compose' nor 'docker-compose' is available"
    print_error "Please install Docker Desktop for macOS"
    exit 1
fi

if [ $? -eq 0 ]; then
    print_status "Monitoring stack started successfully ✓"
else
    print_error "Failed to start monitoring stack ✗"
    exit 1
fi

# Wait for services to be ready
print_step "3. Waiting for services to be ready..."
sleep 10

# Check if services are healthy
print_step "4. Checking service health..."

# Check Prometheus
if curl -s http://localhost:9090/-/healthy > /dev/null; then
    print_status "Prometheus is healthy ✓"
else
    print_warning "Prometheus might not be ready yet"
fi

# Check Grafana
if curl -s http://localhost:3000/api/health > /dev/null; then
    print_status "Grafana is healthy ✓"
else
    print_warning "Grafana might not be ready yet"
fi

# Go back to project root
cd "$PROJECT_DIR"

print_step "5. Starting .NET API..."
# Start the API in the background
nohup dotnet run --project src/Bwadl.Accounting.API > api.log 2>&1 &
API_PID=$!

# Wait a moment for the API to start
sleep 5

# Check if API is running
if curl -s http://localhost:5000/health > /dev/null; then
    print_status ".NET API is healthy ✓"
else
    print_warning ".NET API might not be ready yet"
fi

# Display service URLs
echo ""
echo "🎉 Monitoring Stack is ready!"
echo ""
echo "📊 Access your monitoring services:"
echo "   • Grafana Dashboard: http://localhost:3000"
echo "     Username: admin"
echo "     Password: admin123"
echo ""
echo "   • Prometheus: http://localhost:9090"
echo "   • AlertManager: http://localhost:9093"
echo "   • API Health Checks: http://localhost:5000/health"
echo "   • API Health UI: http://localhost:5000/health-ui"
echo "   • API Metrics: http://localhost:5000/metrics"
echo ""
echo "📱 API Endpoints:"
echo "   • Swagger: http://localhost:5000/swagger"
echo "   • Users API: http://localhost:5000/api/v1/users"
echo "   • Auth API: http://localhost:5000/api/v1/auth"
echo ""
echo "🔍 Logs:"
echo "   • API Log: tail -f api.log"
echo "   • Docker Logs: docker-compose -f monitoring/docker-compose.monitoring.yml logs -f"
echo ""
echo "🛑 To stop all services:"
echo "   • Stop API: kill $API_PID"
echo "   • Stop Monitoring: cd monitoring && docker-compose -f docker-compose.monitoring.yml down"
echo ""

# Save PID for cleanup
echo $API_PID > .api_pid

print_status "Setup complete! Your monitoring stack is now running."
print_warning "Check the logs if any service is not responding correctly."

# Wait for user input before showing dashboard
echo ""
echo "Press ENTER to open Grafana dashboard in your browser, or Ctrl+C to exit..."
read

# Open Grafana dashboard
if command -v open > /dev/null; then
    open http://localhost:3000
elif command -v xdg-open > /dev/null; then
    xdg-open http://localhost:3000
else
    print_warning "Could not open browser automatically. Please navigate to http://localhost:3000"
fi
