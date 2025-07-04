using Bwadl.Accounting.Application.Features.Currencies.DTOs;
using Bwadl.Accounting.Application.Features.Currencies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bwadl.Accounting.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CurrenciesController : ControllerBase
{
    private readonly ICurrencyService _currencyService;
    private readonly ILogger<CurrenciesController> _logger;

    public CurrenciesController(ICurrencyService currencyService, ILogger<CurrenciesController> logger)
    {
        _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all current versions of currencies
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CurrencyDto>>> GetAllCurrentVersions()
    {
        try
        {
            var currencies = await _currencyService.GetAllCurrentVersionsAsync();
            return Ok(currencies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all current currency versions");
            return StatusCode(500, "An error occurred while retrieving currencies");
        }
    }

    /// <summary>
    /// Get current version of a specific currency
    /// </summary>
    [HttpGet("{currencyCode}")]
    public async Task<ActionResult<CurrencyDto>> GetCurrentVersion(string currencyCode)
    {
        try
        {
            var currency = await _currencyService.GetCurrentVersionAsync(currencyCode);
            if (currency == null)
            {
                return NotFound($"Currency with code '{currencyCode}' not found");
            }
            return Ok(currency);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving currency {CurrencyCode}", currencyCode);
            return StatusCode(500, "An error occurred while retrieving the currency");
        }
    }

    /// <summary>
    /// Get all versions of a specific currency
    /// </summary>
    [HttpGet("{currencyCode}/versions")]
    public async Task<ActionResult<List<CurrencyDto>>> GetAllVersions(string currencyCode)
    {
        try
        {
            var currencies = await _currencyService.GetAllVersionsAsync(currencyCode);
            return Ok(currencies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all versions of currency {CurrencyCode}", currencyCode);
            return StatusCode(500, "An error occurred while retrieving currency versions");
        }
    }

    /// <summary>
    /// Create a new currency
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CurrencyDto>> Create([FromBody] CreateCurrencyRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = GetCurrentUser();
            var currency = await _currencyService.CreateAsync(request, currentUser);
            
            return CreatedAtAction(
                nameof(GetCurrentVersion), 
                new { currencyCode = currency.CurrencyCode }, 
                currency);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating currency {CurrencyCode}", request.CurrencyCode);
            return StatusCode(500, "An error occurred while creating the currency");
        }
    }

    /// <summary>
    /// Update an existing currency (creates a new version)
    /// </summary>
    [HttpPut("{currencyCode}")]
    public async Task<ActionResult<CurrencyDto>> Update(string currencyCode, [FromBody] UpdateCurrencyRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = GetCurrentUser();
            var currency = await _currencyService.UpdateAsync(currencyCode, request, currentUser);
            
            return Ok(currency);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating currency {CurrencyCode}", currencyCode);
            return StatusCode(500, "An error occurred while updating the currency");
        }
    }

    /// <summary>
    /// Check if a currency exists
    /// </summary>
    [HttpHead("{currencyCode}")]
    public async Task<ActionResult> Exists(string currencyCode)
    {
        try
        {
            var exists = await _currencyService.ExistsAsync(currencyCode);
            return exists ? Ok() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of currency {CurrencyCode}", currencyCode);
            return StatusCode(500, "An error occurred while checking currency existence");
        }
    }

    private string GetCurrentUser()
    {
        return User?.FindFirstValue(ClaimTypes.Name) 
            ?? User?.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? "System";
    }
}
