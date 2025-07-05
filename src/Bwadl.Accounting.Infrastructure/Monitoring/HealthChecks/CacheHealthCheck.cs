using Microsoft.Extensions.Diagnostics.HealthChecks;
using Bwadl.Accounting.Application.Common.Interfaces;

namespace Bwadl.Accounting.Infrastructure.Monitoring.HealthChecks;

public class CacheHealthCheck : IHealthCheck
{
    private readonly ICacheService _cacheService;

    public CacheHealthCheck(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var testKey = "health-check-test";
            var testValue = "test-value";
            
            await _cacheService.SetAsync(testKey, testValue, TimeSpan.FromSeconds(10), cancellationToken);
            var retrievedValue = await _cacheService.GetAsync<string>(testKey, cancellationToken);
            await _cacheService.RemoveAsync(testKey, cancellationToken);
            
            var data = new Dictionary<string, object>
            {
                { "cache_type", _cacheService.GetType().Name },
                { "test_successful", retrievedValue == testValue }
            };
            
            return retrievedValue == testValue 
                ? HealthCheckResult.Healthy("Cache is healthy", data)
                : HealthCheckResult.Degraded("Cache is not working properly", data: data);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Cache connection failed", ex);
        }
    }
}
