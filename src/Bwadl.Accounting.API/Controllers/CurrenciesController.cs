using Asp.Versioning;
using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.API.Models.Currencies;
using Bwadl.Accounting.Application.Features.Currencies.Commands.CreateCurrency;
using Bwadl.Accounting.Application.Features.Currencies.Commands.UpdateCurrency;
using Bwadl.Accounting.Application.Features.Currencies.Commands.DeleteCurrency;
using Bwadl.Accounting.Application.Features.Currencies.Queries.GetCurrency;
using Bwadl.Accounting.Application.Features.Currencies.Queries.GetAllCurrencies;
using Bwadl.Accounting.Application.Features.Currencies.Queries.GetCurrencyVersions;
using Bwadl.Accounting.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bwadl.Accounting.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/currencies")]
[Produces("application/json")]
public class CurrenciesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CurrenciesController> _logger;

    public CurrenciesController(IMediator mediator, ILogger<CurrenciesController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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
            var query = new GetAllCurrenciesQuery();
            var currencies = await _mediator.Send(query);
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
            var query = new GetCurrencyQuery(currencyCode);
            var currency = await _mediator.Send(query);
            
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
            var query = new GetCurrencyVersionsQuery(currencyCode);
            var currencies = await _mediator.Send(query);
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
    public async Task<ActionResult<CurrencyDto>> Create([FromBody] Models.Currencies.CreateCurrencyRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = GetCurrentUser();
            var command = new CreateCurrencyCommand(
                request.CurrencyCode,
                request.CurrencyName,
                request.DecimalPlaces,
                currentUser
            );

            var currency = await _mediator.Send(command);
            
            return CreatedAtAction(
                nameof(GetCurrentVersion), 
                new { currencyCode = currency.CurrencyCode }, 
                currency);
        }
        catch (DuplicateCurrencyCodeException ex)
        {
            return Conflict(ex.Message);
        }
        catch (InvalidCurrencyCodeException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidDecimalPlacesException ex)
        {
            return BadRequest(ex.Message);
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
    public async Task<ActionResult<CurrencyDto>> Update(string currencyCode, [FromBody] Models.Currencies.UpdateCurrencyRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = GetCurrentUser();
            var command = new UpdateCurrencyCommand(
                currencyCode,
                request.CurrencyName,
                request.DecimalPlaces,
                currentUser
            );

            var currency = await _mediator.Send(command);
            return Ok(currency);
        }
        catch (CurrencyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidDecimalPlacesException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating currency {CurrencyCode}", currencyCode);
            return StatusCode(500, "An error occurred while updating the currency");
        }
    }

    /// <summary>
    /// Delete a currency
    /// </summary>
    [HttpDelete("{currencyCode}")]
    public async Task<ActionResult> Delete(string currencyCode)
    {
        try
        {
            var command = new DeleteCurrencyCommand(currencyCode);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (CurrencyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (CurrencyInUseException ex)
        {
            return Conflict(ex.Message);
        }
        catch (BaseCurrencyException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting currency {CurrencyCode}", currencyCode);
            return StatusCode(500, "An error occurred while deleting the currency");
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
            var query = new GetCurrencyQuery(currencyCode);
            var currency = await _mediator.Send(query);
            return currency != null ? Ok() : NotFound();
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
