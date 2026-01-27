using Gck.Application.DTOs;
using Gck.Application.Features.HourlyFees.Queries.GetAllHourlyFees;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gck.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HourlyFeesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<HourlyFeesController> _logger;

    public HourlyFeesController(IMediator mediator, ILogger<HourlyFeesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all hourly fees
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<HourlyFeeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<HourlyFeeDto>>> GetAllHourlyFees()
    {
        try
        {
            var fees = await _mediator.Send(new GetAllHourlyFeesQuery());
            return Ok(fees);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hourly fees");
            return StatusCode(500, "An error occurred while retrieving hourly fees");
        }
    }
}
