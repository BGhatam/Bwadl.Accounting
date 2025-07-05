# 🚀 Quick Start - Prometheus & Grafana Monitoring Setup

## One-Command Setup

```bash
# Navigate to your project directory
cd /Users/baqeralghatam/workspace/backend/dotnet/Bwadl.Accounting

# Start everything
./monitoring/start-monitoring.sh
```

## What Gets Started

### 🐳 **Docker Services**
- **Prometheus** (port 9090) - Metrics collection
- **Grafana** (port 3000) - Dashboards and visualization  
- **Alertmanager** (port 9093) - Alert management
- **Node Exporter** (port 9100) - System metrics

### 🔧 **.NET Application**
- **API** (port 5000) - Your Bwadl Accounting API with metrics enabled

## 📊 Access Your Monitoring

| Service | URL | Credentials |
|---------|-----|-------------|
| **Grafana Dashboard** | http://localhost:3000 | admin / admin123 |
| **Prometheus Web UI** | http://localhost:9090 | None |
| **Alertmanager** | http://localhost:9093 | None |
| **API Health Checks** | http://localhost:5000/health | None |
| **API Metrics** | http://localhost:5000/metrics | None |

## 🎯 What You'll See

### **Grafana Dashboard Features**
- ✅ **Real-time API health status**
- 📈 **Request rate and response times**
- 💾 **Memory and CPU usage**
- 🚨 **Error rates with automatic alerts**
- 💼 **Business metrics** (users created, auth attempts, etc.)
- 🖥️ **System resource monitoring**

### **Health Check Endpoints**
- `/health` - Overall health status
- `/health/live` - Kubernetes liveness probe
- `/health/ready` - Kubernetes readiness probe
- `/health/detailed` - Detailed health information
- `/health-ui` - Visual health dashboard

### **Prometheus Metrics**
- HTTP request metrics (rate, duration, status codes)
- .NET runtime metrics (memory, GC, threads)
- Custom business metrics (users, auth, emails)
- Health check status metrics

## 🛑 Stop Everything

```bash
./monitoring/stop-monitoring.sh
```

## 📋 Prerequisites

- **Docker Desktop** - Must be running
- **.NET 8 SDK** - For running the API
- **macOS** - This setup is optimized for macOS

## 🔧 Customization

### **Add Custom Metrics**
Edit `src/Bwadl.Accounting.Infrastructure/Monitoring/PrometheusMetrics.cs`:

```csharp
public static readonly Counter MyCustomMetric = Metrics
    .CreateCounter("bwadl_my_custom_total", "Description", new[] { "label" });
```

### **Modify Alerts**
Edit `monitoring/prometheus/alert.rules.yml` to adjust thresholds:

```yaml
- alert: HighMemoryUsage
  expr: (process_working_set_bytes / 1024 / 1024) > 500  # Change threshold
```

### **Update Email Notifications**
Edit `monitoring/alertmanager/alertmanager.yml`:

```yaml
receivers:
  - name: 'default-receiver'
    email_configs:
      - to: 'your-email@company.com'  # Change email
```

## 🚨 Troubleshooting

### **Services Not Starting**
```bash
# Check Docker is running
docker info

# Check logs
docker-compose -f monitoring/docker-compose.monitoring.yml logs -f

# Restart specific service
docker restart bwadl-prometheus
```

### **API Not Accessible**
```bash
# Check if API is running
curl http://localhost:5000/health

# Check application logs
tail -f logs/api-*.txt

# Restart API
pkill -f "dotnet.*Bwadl.Accounting.API"
dotnet run --project src/Bwadl.Accounting.API
```

### **No Metrics in Grafana**
1. Check Prometheus targets: http://localhost:9090/targets
2. Verify API metrics endpoint: http://localhost:5000/metrics
3. Check Grafana data source: Configuration → Data Sources → Prometheus

## 📈 Monitoring Best Practices

### **What to Monitor**
1. **Golden Signals**: Latency, Traffic, Errors, Saturation
2. **Health Checks**: Database, Cache, External Services  
3. **Business Metrics**: User registrations, Authentication attempts
4. **System Resources**: Memory, CPU, Disk space

### **Alert Configuration**
- **Critical alerts**: API down, Database unavailable
- **Warning alerts**: High memory usage, Slow response times
- **Info alerts**: High request volume, New deployments

### **Dashboard Organization**
1. **Overview**: High-level system health
2. **Performance**: Response times, throughput
3. **Errors**: Error rates, failed requests
4. **Resources**: Memory, CPU, disk usage
5. **Business**: User activity, feature usage

## 🔒 Production Considerations

### **Security**
- Change default Grafana password
- Enable HTTPS for all services
- Restrict network access
- Use proper authentication

### **Data Retention**
- Configure Prometheus retention (currently 30 days)
- Set up remote storage for long-term data
- Regular backup of Grafana dashboards

### **Scalability**
- Use Prometheus federation for multiple instances
- Implement high availability for Grafana
- Consider Prometheus remote write for large scale

## 📚 Additional Resources

- [Prometheus Documentation](https://prometheus.io/docs/)
- [Grafana Documentation](https://grafana.com/docs/)
- [.NET Metrics Guide](https://docs.microsoft.com/en-us/dotnet/core/diagnostics/metrics)
- [Kubernetes Monitoring](https://kubernetes.io/docs/tasks/debug-application-cluster/resource-usage-monitoring/)

---

🎉 **Enjoy monitoring your Bwadl Accounting API!** 

For questions or issues, check the logs or refer to the comprehensive setup guide in `PROMETHEUS_GRAFANA_SETUP_GUIDE.md`.
