#!/bin/bash

# Bwadl Accounting - Monitoring Stack Startup Script
# This script starts the complete monitoring stack for your .NET application

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Docker is running
check_docker() {
    if ! docker info > /dev/null 2>&1; then
        print_error "Docker is not running. Please start Docker Desktop first."
        exit 1
    fi
    print_success "Docker is running"
}

# Check if required files exist
check_files() {
    if [ ! -f "monitoring/docker-compose.monitoring.yml" ]; then
        print_error "docker-compose.monitoring.yml not found in monitoring directory"
        exit 1
    fi
    print_success "All required files found"
}

# Start monitoring stack
start_monitoring() {
    print_status "Starting monitoring stack..."
    cd monitoring
    docker-compose -f docker-compose.monitoring.yml up -d
    cd ..
    print_success "Monitoring stack started"
}

# Wait for services to be ready
wait_for_services() {
    print_status "Waiting for services to be ready..."
    
    # Wait for Prometheus
    print_status "Waiting for Prometheus..."
    until curl -s http://localhost:9090/-/ready > /dev/null 2>&1; do
        sleep 2
        echo -n "."
    done
    print_success "Prometheus is ready"
    
    # Wait for Grafana
    print_status "Waiting for Grafana..."
    until curl -s http://localhost:3000/api/health > /dev/null 2>&1; do
        sleep 2
        echo -n "."
    done
    print_success "Grafana is ready"
    
    # Wait for Alertmanager
    print_status "Waiting for Alertmanager..."
    until curl -s http://localhost:9093/-/ready > /dev/null 2>&1; do
        sleep 2
        echo -n "."
    done
    print_success "Alertmanager is ready"
}

# Start .NET application
start_dotnet_app() {
    print_status "Starting .NET application..."
    # Start the application in background
    dotnet run --project src/Bwadl.Accounting.API &
    DOTNET_PID=$!
    
    # Wait for the application to start
    print_status "Waiting for .NET application to start..."
    sleep 10
    
    # Check if the application is running
    if curl -s http://localhost:5000/health > /dev/null 2>&1; then
        print_success ".NET application is running (PID: $DOTNET_PID)"
        echo $DOTNET_PID > .dotnet.pid
    else
        print_error "Failed to start .NET application"
        exit 1
    fi
}

# Display access information
display_info() {
    echo ""
    print_success "🎉 Monitoring stack is ready!"
    echo ""
    echo "📊 Access URLs:"
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo "🔍 Grafana Dashboard:     http://localhost:3000"
    echo "   Username: admin, Password: admin123"
    echo ""
    echo "📈 Prometheus Web UI:     http://localhost:9090"
    echo "🚨 Alertmanager:          http://localhost:9093"
    echo "💾 Node Exporter:         http://localhost:9100"
    echo ""
    echo "🏥 Your API Health Checks:"
    echo "   General Health:        http://localhost:5000/health"
    echo "   Detailed Health:       http://localhost:5000/health/detailed"
    echo "   Liveness Probe:        http://localhost:5000/health/live"
    echo "   Readiness Probe:       http://localhost:5000/health/ready"
    echo "   Health UI:             http://localhost:5000/health-ui"
    echo ""
    echo "📊 Prometheus Metrics:     http://localhost:5000/metrics"
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo ""
    echo "🛑 To stop everything:"
    echo "   ./monitoring/stop-monitoring.sh"
}

# Create stop script
create_stop_script() {
    cat > monitoring/stop-monitoring.sh << 'EOF'
#!/bin/bash

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m'

print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

# Stop .NET application
if [ -f "../.dotnet.pid" ]; then
    DOTNET_PID=$(cat ../.dotnet.pid)
    print_status "Stopping .NET application (PID: $DOTNET_PID)..."
    kill $DOTNET_PID 2>/dev/null || echo "Process already stopped"
    rm -f ../.dotnet.pid
    print_success ".NET application stopped"
fi

# Stop monitoring stack
print_status "Stopping monitoring stack..."
docker-compose -f docker-compose.monitoring.yml down

print_success "All services stopped"
EOF

    chmod +x monitoring/stop-monitoring.sh
}

# Main execution
main() {
    echo "🚀 Bwadl Accounting - Monitoring Stack Setup"
    echo "=============================================="
    
    check_docker
    check_files
    start_monitoring
    wait_for_services
    start_dotnet_app
    create_stop_script
    display_info
}

# Run if script is executed directly
if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    main "$@"
fi
