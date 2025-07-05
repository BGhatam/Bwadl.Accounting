#!/bin/bash

# Bwadl API Monitoring Stack Stop Script
# This script will stop your .NET API, Prometheus, Grafana, and all monitoring components

set -e

echo "🛑 Stopping Bwadl API Monitoring Stack..."

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

# Navigate to project directory
PROJECT_DIR="/Users/baqeralghatam/workspace/backend/dotnet/Bwadl.Accounting"
cd "$PROJECT_DIR"

print_step "1. Stopping .NET API..."

# Check if API PID file exists and stop the process
if [ -f .api_pid ]; then
    API_PID=$(cat .api_pid)
    if ps -p $API_PID > /dev/null; then
        kill $API_PID
        print_status "API process stopped (PID: $API_PID) ✓"
    else
        print_warning "API process was not running"
    fi
    rm -f .api_pid
else
    print_warning "API PID file not found, attempting to find and kill dotnet processes"
    # Try to find and kill any dotnet run processes for this project
    pkill -f "dotnet.*Bwadl.Accounting.API" || print_warning "No API processes found"
fi

print_step "2. Stopping Monitoring Stack..."
cd monitoring

# Stop and remove containers using modern docker compose or fallback
if command -v "docker" >/dev/null 2>&1 && docker compose version >/dev/null 2>&1; then
    print_status "Using modern 'docker compose' command"
    docker compose -f docker-compose.monitoring.yml down
elif command -v "docker-compose" >/dev/null 2>&1; then
    print_status "Using legacy 'docker-compose' command"
    docker-compose -f docker-compose.monitoring.yml down
else
    print_warning "Docker Compose not available, attempting manual container cleanup"
    docker stop bwadl-prometheus bwadl-grafana bwadl-alertmanager bwadl-node-exporter 2>/dev/null || true
    docker rm bwadl-prometheus bwadl-grafana bwadl-alertmanager bwadl-node-exporter 2>/dev/null || true
fi

if [ $? -eq 0 ]; then
    print_status "Monitoring stack stopped successfully ✓"
else
    print_error "Failed to stop monitoring stack ✗"
fi

# Go back to project root
cd "$PROJECT_DIR"

print_step "3. Cleaning up logs..."
if [ -f api.log ]; then
    rm api.log
    print_status "API log file removed ✓"
fi

print_step "4. Checking for remaining processes..."

# Check if any services are still running
PROMETHEUS_RUNNING=$(docker ps -q -f name=bwadl-prometheus)
GRAFANA_RUNNING=$(docker ps -q -f name=bwadl-grafana)
API_RUNNING=$(ps aux | grep -v grep | grep "dotnet.*Bwadl.Accounting.API" | wc -l)

if [ ! -z "$PROMETHEUS_RUNNING" ]; then
    print_warning "Prometheus container is still running"
fi

if [ ! -z "$GRAFANA_RUNNING" ]; then
    print_warning "Grafana container is still running"
fi

if [ $API_RUNNING -gt 0 ]; then
    print_warning "API processes are still running"
fi

echo ""
print_status "🎉 Monitoring stack shutdown complete!"
echo ""
print_status "All services have been stopped:"
echo "   • .NET API"
echo "   • Prometheus"
echo "   • Grafana"
echo "   • AlertManager"
echo "   • Node Exporter"
echo ""

# Optionally remove Docker volumes (ask user)
echo "Do you want to remove monitoring data volumes? (y/N)"
read -r response
if [[ "$response" =~ ^([yY][eE][sS]|[yY])$ ]]; then
    print_step "Removing Docker volumes..."
    cd monitoring
    docker-compose -f docker-compose.monitoring.yml down -v
    print_status "Docker volumes removed ✓"
    print_warning "All monitoring data has been deleted!"
else
    print_status "Docker volumes preserved. Data will be available on next startup."
fi

echo ""
print_status "To restart the monitoring stack, run: ./start-monitoring.sh"
