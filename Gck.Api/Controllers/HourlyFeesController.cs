using Gck.Application.DTOs;
using Gck.Application.Features.HourlyFees.Commands.CreateHourlyFee;
using Gck.Application.Features.HourlyFees.Commands.DeleteHourlyFee;
using Gck.Application.Features.HourlyFees.Commands.UpdateHourlyFee;
using Gck.Application.Features.HourlyFees.Queries.GetAllHourlyFees;
using Gck.Application.Features.HourlyFees.Queries.GetHourlyFeeById;
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
    public async Task<ActionResult<List<HourlyFeeDto>>> GetAll()
    {
        var fees = await _mediator.Send(new GetAllHourlyFeesQuery());
        return Ok(fees);
    }

    /// <summary>
    /// Get hourly fee by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HourlyFeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HourlyFeeDto>> GetById(int id)
    {
        var fee = await _mediator.Send(new GetHourlyFeeByIdQuery { Id = id });
        if (fee == null)
        {
            return NotFound($"HourlyFee with ID '{id}' not found");
        }
        return Ok(fee);
    }

    /// <summary>
    /// Create a new hourly fee
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create([FromBody] CreateHourlyFeeCommand command)
    {
        var feeId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = feeId }, feeId);
    }

    /// <summary>
    /// Update an existing hourly fee
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateHourlyFeeCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID in URL does not match ID in request body");
        }

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Delete an hourly fee
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteHourlyFeeCommand { Id = id });
        return NoContent();
    }
}
