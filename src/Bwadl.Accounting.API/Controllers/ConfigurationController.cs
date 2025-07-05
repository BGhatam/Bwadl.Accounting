using Asp.Versioning;
using Bwadl.Accounting.Infrastructure.Configuration;
using Bwadl.Accounting.Shared.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Bwadl.Accounting.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/configuration")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfigurationService _configurationService;
    private readonly ILogger<ConfigurationController> _logger;

    public ConfigurationController(
        IConfigurationService configurationService,
        ILogger<ConfigurationController> logger)
    {
        _configurationService = configurationService;
        _logger = logger;
    }

    [HttpGet("application")]
    public async Task<ActionResult<ApplicationOptions>> GetApplicationConfiguration(CancellationToken cancellationToken)
    {
        try
        {
            var options = await _configurationService.GetOptionsAsync<ApplicationOptions>(cancellationToken);
            return Ok(options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving application configuration");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("features")]
    public async Task<ActionResult<FeatureOptions>> GetFeatureConfiguration(CancellationToken cancellationToken)
    {
        try
        {
            var options = await _configurationService.GetOptionsAsync<FeatureOptions>(cancellationToken);
            return Ok(options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving feature configuration");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("cache")]
    public async Task<ActionResult<CacheOptions>> GetCacheConfiguration(CancellationToken cancellationToken)
    {
        try
        {
            var options = await _configurationService.GetOptionsAsync<CacheOptions>(cancellationToken);
            return Ok(options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cache configuration");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("security")]
    public async Task<ActionResult<SecurityOptions>> GetSecurityConfiguration(CancellationToken cancellationToken)
    {
        try
        {
            var options = await _configurationService.GetSecurityOptionsWithSecretsAsync(cancellationToken);
            
            // Never return the actual secret key in the response
            options.Jwt.SecretKey = "[REDACTED]";
            
            return Ok(options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving security configuration");
            return StatusCode(500, "Internal server error");
        }
    }
}
