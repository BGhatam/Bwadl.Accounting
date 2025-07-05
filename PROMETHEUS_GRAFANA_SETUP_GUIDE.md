# Complete Prometheus & Grafana Setup Guide for .NET Application

## Overview
This guide will set up a complete monitoring stack with:
- **Prometheus** - Metrics collection and storage
- **Grafana** - Visualization and dashboards
- **.NET Application Metrics** - Custom metrics from your API
- **Health Check Integration** - Monitor your health endpoints
- **Alerting** - Notifications for critical issues

## Prerequisites
- macOS (your current OS)
- Docker Desktop installed
- Your .NET application running
- Terminal access

---

## Part 1: Download and Install Required Software

### 1.1 Install Docker Desktop (if not already installed)
```bash
# Download from: https://www.docker.com/products/docker-desktop/
# Or install via Homebrew:
brew install --cask docker
```

### 1.2 Install Homebrew (if not already installed)
```bash
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
```

### 1.3 Install Prometheus (Local Installation)
```bash
# Install Prometheus via Homebrew
brew install prometheus

# Or download manually from: https://prometheus.io/download/
```

### 1.4 Install Grafana (Local Installation)
```bash
# Install Grafana via Homebrew
brew install grafana

# Or download from: https://grafana.com/grafana/download?platform=mac
```

---

## Part 2: Docker Compose Setup (Recommended Approach)

Create a complete monitoring stack using Docker Compose for easier management.

### 2.1 Create Monitoring Directory Structure
```bash
cd /Users/baqeralghatam/workspace/backend/dotnet/Bwadl.Accounting
mkdir -p monitoring/{prometheus,grafana,alertmanager}
cd monitoring
```

### 2.2 Create Docker Compose File
```yaml
# docker-compose.monitoring.yml
version: '3.8'

services:
  prometheus:
    image: prom/prometheus:latest
    container_name: bwadl-prometheus
    ports:
      - "9090:9090"
    volumes:
      - ./prometheus/prometheus.yml:/etc/prometheus/prometheus.yml
      - ./prometheus/alert.rules.yml:/etc/prometheus/alert.rules.yml
      - prometheus_data:/prometheus
    command:
      - '--config.file=/etc/prometheus/prometheus.yml'
      - '--storage.tsdb.path=/prometheus'
      - '--web.console.libraries=/etc/prometheus/console_libraries'
      - '--web.console.templates=/etc/prometheus/consoles'
      - '--storage.tsdb.retention.time=30d'
      - '--web.enable-lifecycle'
      - '--web.enable-admin-api'
    networks:
      - monitoring
    restart: unless-stopped

  grafana:
    image: grafana/grafana:latest
    container_name: bwadl-grafana
    ports:
      - "3000:3000"
    volumes:
      - ./grafana/provisioning:/etc/grafana/provisioning
      - ./grafana/dashboards:/var/lib/grafana/dashboards
      - grafana_data:/var/lib/grafana
    environment:
      - GF_SECURITY_ADMIN_PASSWORD=admin123
      - GF_USERS_ALLOW_SIGN_UP=false
      - GF_PATHS_PROVISIONING=/etc/grafana/provisioning
    networks:
      - monitoring
    restart: unless-stopped
    depends_on:
      - prometheus

  alertmanager:
    image: prom/alertmanager:latest
    container_name: bwadl-alertmanager
    ports:
      - "9093:9093"
    volumes:
      - ./alertmanager/alertmanager.yml:/etc/alertmanager/alertmanager.yml
      - alertmanager_data:/alertmanager
    command:
      - '--config.file=/etc/alertmanager/alertmanager.yml'
      - '--storage.path=/alertmanager'
      - '--web.external-url=http://localhost:9093'
    networks:
      - monitoring
    restart: unless-stopped

  node-exporter:
    image: prom/node-exporter:latest
    container_name: bwadl-node-exporter
    ports:
      - "9100:9100"
    volumes:
      - /proc:/host/proc:ro
      - /sys:/host/sys:ro
      - /:/rootfs:ro
    command:
      - '--path.procfs=/host/proc'
      - '--path.rootfs=/rootfs'
      - '--path.sysfs=/host/sys'
      - '--collector.filesystem.mount-points-exclude=^/(sys|proc|dev|host|etc)($$|/)'
    networks:
      - monitoring
    restart: unless-stopped

volumes:
  prometheus_data:
  grafana_data:
  alertmanager_data:

networks:
  monitoring:
    driver: bridge
```

### 2.3 Create Prometheus Configuration
```yaml
# prometheus/prometheus.yml
global:
  scrape_interval: 15s
  evaluation_interval: 15s

rule_files:
  - "alert.rules.yml"

alerting:
  alertmanagers:
    - static_configs:
        - targets:
          - alertmanager:9093

scrape_configs:
  # Prometheus itself
  - job_name: 'prometheus'
    static_configs:
      - targets: ['localhost:9090']

  # Your .NET Application
  - job_name: 'bwadl-api'
    static_configs:
      - targets: ['host.docker.internal:5000']  # Adjust port as needed
    metrics_path: '/metrics'
    scrape_interval: 10s

  # Health Checks
  - job_name: 'bwadl-health'
    static_configs:
      - targets: ['host.docker.internal:5000']
    metrics_path: '/health'
    scrape_interval: 30s

  # Node Exporter (System Metrics)
  - job_name: 'node-exporter'
    static_configs:
      - targets: ['node-exporter:9100']

  # Additional health endpoints
  - job_name: 'bwadl-health-detailed'
    static_configs:
      - targets: ['host.docker.internal:5000']
    metrics_path: '/health/detailed'
    scrape_interval: 60s
```

### 2.4 Create Alert Rules
```yaml
# prometheus/alert.rules.yml
groups:
  - name: bwadl-api-alerts
    rules:
      - alert: APIDown
        expr: up{job="bwadl-api"} == 0
        for: 1m
        labels:
          severity: critical
        annotations:
          summary: "Bwadl API is down"
          description: "The Bwadl API has been down for more than 1 minute."

      - alert: HighMemoryUsage
        expr: (process_working_set_bytes / 1024 / 1024) > 500
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "High memory usage detected"
          description: "Memory usage is above 500MB for more than 5 minutes."

      - alert: HealthCheckFailed
        expr: aspnetcore_healthcheck_status != 1
        for: 2m
        labels:
          severity: critical
        annotations:
          summary: "Health check failed"
          description: "One or more health checks are failing."

      - alert: HighRequestRate
        expr: rate(http_requests_received_total[5m]) > 100
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "High request rate"
          description: "Request rate is above 100 requests per second."
```

### 2.5 Create Alertmanager Configuration
```yaml
# alertmanager/alertmanager.yml
global:
  smtp_smarthost: 'localhost:587'
  smtp_from: 'alerts@bwadl.com'

route:
  group_by: ['alertname']
  group_wait: 10s
  group_interval: 10s
  repeat_interval: 1h
  receiver: 'default-receiver'

receivers:
  - name: 'default-receiver'
    email_configs:
      - to: 'admin@bwadl.com'
        subject: 'Bwadl Alert: {{ .GroupLabels.alertname }}'
        body: |
          {{ range .Alerts }}
          Alert: {{ .Annotations.summary }}
          Description: {{ .Annotations.description }}
          {{ end }}
    # Slack notifications (optional)
    slack_configs:
      - api_url: 'YOUR_SLACK_WEBHOOK_URL'
        channel: '#alerts'
        title: 'Bwadl Alert'
        text: '{{ range .Alerts }}{{ .Annotations.summary }}{{ end }}'
```

---

## Part 3: Configure Your .NET Application for Prometheus

### 3.1 Install Required NuGet Packages
```bash
cd /Users/baqeralghatam/workspace/backend/dotnet/Bwadl.Accounting

# Add Prometheus packages
dotnet add src/Bwadl.Accounting.API/Bwadl.Accounting.API.csproj package prometheus-net.AspNetCore
dotnet add src/Bwadl.Accounting.API/Bwadl.Accounting.API.csproj package prometheus-net.SystemMetrics
```

### 3.2 Update Program.cs
```csharp
// Add these imports at the top
using Prometheus;

// In your Program.cs, add after other services:
builder.Services.AddSingleton<IMetricsLogger>(KestrelMetricServer.DefaultRegistry);

var app = builder.Build();

// Add Prometheus middleware (add this before other middleware)
app.UseMetricServer(); // Exposes /metrics endpoint
app.UseHttpMetrics();  // Collects HTTP metrics

// ... rest of your middleware configuration
```

### 3.3 Create Custom Metrics Service
```csharp
// Infrastructure/Monitoring/PrometheusMetrics.cs
using Prometheus;

namespace Bwadl.Accounting.Infrastructure.Monitoring;

public static class PrometheusMetrics
{
    // Custom business metrics
    public static readonly Counter UsersCreated = Metrics
        .CreateCounter("bwadl_users_created_total", "Total number of users created");

    public static readonly Counter AuthenticationAttempts = Metrics
        .CreateCounter("bwadl_auth_attempts_total", "Total authentication attempts", new[] { "result" });

    public static readonly Histogram RequestDuration = Metrics
        .CreateHistogram("bwadl_request_duration_seconds", "Request duration in seconds", 
        new[] { "method", "endpoint", "status_code" });

    public static readonly Gauge ActiveConnections = Metrics
        .CreateGauge("bwadl_active_connections", "Number of active connections");

    public static readonly Counter EmailsSent = Metrics
        .CreateCounter("bwadl_emails_sent_total", "Total emails sent", new[] { "type", "status" });

    // Health check metrics
    public static readonly Gauge HealthCheckStatus = Metrics
        .CreateGauge("bwadl_health_check_status", "Health check status (1=healthy, 0=unhealthy)", 
        new[] { "check_name" });
}
```

### 3.4 Update Health Checks to Export Metrics
```csharp
// Update your health checks to report to Prometheus
// In DatabaseHealthCheck.cs, add at the end of CheckHealthAsync:

var isHealthy = result.Status == HealthStatus.Healthy ? 1 : 0;
PrometheusMetrics.HealthCheckStatus.WithLabels("database").Set(isHealthy);
```

---

## Part 4: Create Grafana Dashboards

### 4.1 Create Provisioning Configuration
```yaml
# grafana/provisioning/datasources/prometheus.yml
apiVersion: 1

datasources:
  - name: Prometheus
    type: prometheus
    access: proxy
    url: http://prometheus:9090
    isDefault: true
    editable: true
```

### 4.2 Create Dashboard Configuration
```yaml
# grafana/provisioning/dashboards/dashboard.yml
apiVersion: 1

providers:
  - name: 'bwadl-dashboards'
    orgId: 1
    folder: ''
    type: file
    disableDeletion: false
    updateIntervalSeconds: 10
    allowUiUpdates: true
    options:
      path: /var/lib/grafana/dashboards
```

### 4.3 Create Custom Dashboard JSON
```json
{
  "dashboard": {
    "id": null,
    "title": "Bwadl API Monitoring",
    "tags": ["bwadl", "api"],
    "timezone": "browser",
    "panels": [
      {
        "id": 1,
        "title": "API Health Status",
        "type": "stat",
        "targets": [
          {
            "expr": "up{job=\"bwadl-api\"}",
            "legendFormat": "API Status"
          }
        ],
        "fieldConfig": {
          "defaults": {
            "mappings": [
              {"options": {"0": {"text": "DOWN"}}, "type": "value"},
              {"options": {"1": {"text": "UP"}}, "type": "value"}
            ],
            "thresholds": {
              "steps": [
                {"color": "red", "value": 0},
                {"color": "green", "value": 1}
              ]
            }
          }
        }
      },
      {
        "id": 2,
        "title": "Request Rate",
        "type": "graph",
        "targets": [
          {
            "expr": "rate(http_requests_received_total[5m])",
            "legendFormat": "Requests/sec"
          }
        ]
      },
      {
        "id": 3,
        "title": "Response Time",
        "type": "graph",
        "targets": [
          {
            "expr": "histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m]))",
            "legendFormat": "95th percentile"
          }
        ]
      },
      {
        "id": 4,
        "title": "Memory Usage",
        "type": "graph",
        "targets": [
          {
            "expr": "process_working_set_bytes / 1024 / 1024",
            "legendFormat": "Memory (MB)"
          }
        ]
      },
      {
        "id": 5,
        "title": "Health Checks Status",
        "type": "table",
        "targets": [
          {
            "expr": "bwadl_health_check_status",
            "format": "table"
          }
        ]
      }
    ],
    "time": {
      "from": "now-1h",
      "to": "now"
    },
    "refresh": "5s"
  }
}
```

---

## Part 5: Launch and Configure

### 5.1 Start Your .NET Application
```bash
cd /Users/baqeralghatam/workspace/backend/dotnet/Bwadl.Accounting
dotnet run --project src/Bwadl.Accounting.API
```

### 5.2 Start Monitoring Stack
```bash
# In the monitoring directory
cd monitoring
docker-compose -f docker-compose.monitoring.yml up -d
```

### 5.3 Verify Services
```bash
# Check if all containers are running
docker ps

# View logs if needed
docker-compose -f docker-compose.monitoring.yml logs -f
```

---

## Part 6: Access and Configure Dashboards

### 6.1 Access URLs
- **Grafana Dashboard**: http://localhost:3000
  - Username: `admin`
  - Password: `admin123`

- **Prometheus Web UI**: http://localhost:9090
- **Alertmanager**: http://localhost:9093
- **Your API Health**: http://localhost:5000/health
- **Your API Metrics**: http://localhost:5000/metrics

### 6.2 Initial Grafana Setup
1. Open http://localhost:3000
2. Login with admin/admin123
3. Go to "Configuration" → "Data Sources"
4. Verify Prometheus is connected
5. Go to "+" → "Import" → Upload the dashboard JSON
6. Customize panels as needed

### 6.3 Key Metrics to Monitor
- **Availability**: `up` metric
- **Response Time**: `http_request_duration_seconds`
- **Request Rate**: `http_requests_received_total`
- **Error Rate**: `http_requests_received_total{status=~"5.."}` 
- **Memory Usage**: `process_working_set_bytes`
- **Health Checks**: `bwadl_health_check_status`

---

## Part 7: Production Considerations

### 7.1 Security
```yaml
# Add authentication to Prometheus
# prometheus/web.yml
basic_auth_users:
  admin: $2b$12$hNf2lSsxfm0.i4a.1kVpSOVyBCfIB51VRjgBUyv6kdnyTlgWj81Ay  # bcrypt hash
```

### 7.2 Data Retention
```yaml
# Adjust retention in docker-compose
command:
  - '--storage.tsdb.retention.time=90d'  # Increase retention
```

### 7.3 Backup Strategy
```bash
# Backup Grafana dashboards
docker exec bwadl-grafana grafana-cli admin export-dashboard > backup.json

# Backup Prometheus data
docker exec bwadl-prometheus promtool tsdb dump /prometheus
```

---

## Part 8: Troubleshooting

### 8.1 Common Issues
```bash
# Check if .NET app exposes metrics
curl http://localhost:5000/metrics

# Check Prometheus targets
# Go to http://localhost:9090/targets

# Check Grafana logs
docker logs bwadl-grafana

# Test health endpoints
curl http://localhost:5000/health/detailed
```

### 8.2 Useful Commands
```bash
# Restart monitoring stack
docker-compose -f docker-compose.monitoring.yml restart

# Update configuration without restart
curl -X POST http://localhost:9090/-/reload

# Check metric values
curl -s 'http://localhost:9090/api/v1/query?query=up'
```

This comprehensive setup will provide you with enterprise-grade monitoring for your .NET application with health checks, custom metrics, alerting, and beautiful dashboards.
