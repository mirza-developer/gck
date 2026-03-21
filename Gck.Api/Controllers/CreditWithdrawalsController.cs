using Gck.Application.DTOs;
using Gck.Application.Features.CreditWithdrawals.Commands.CreateWithdrawal;
using Gck.Application.Features.CreditWithdrawals.Commands.ProcessWithdrawal;
using Gck.Application.Features.CreditWithdrawals.Queries.GetAllWithdrawals;
using Gck.Application.Features.CreditWithdrawals.Queries.GetCustomerWithdrawals;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gck.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CreditWithdrawalsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreditWithdrawalsController> _logger;

    public CreditWithdrawalsController(IMediator mediator, ILogger<CreditWithdrawalsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CreditWithdrawalRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CreditWithdrawalRequestDto>>> GetAll([FromQuery] string? status = null)
    {
        var requests = await _mediator.Send(new GetAllWithdrawalsQuery { Status = status });
        return Ok(requests);
    }

    [HttpGet("customer/{customerId}")]
    [ProducesResponseType(typeof(List<CreditWithdrawalRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CreditWithdrawalRequestDto>>> GetByCustomer(int customerId)
    {
        var requests = await _mediator.Send(new GetCustomerWithdrawalsQuery { CustomerId = customerId });
        return Ok(requests);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateWithdrawalResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<CreateWithdrawalResult>> Create([FromBody] CreateWithdrawalCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/process")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Process(int id, [FromBody] ProcessWithdrawalCommand command)
    {
        command.RequestId = id;
        await _mediator.Send(command);
        return NoContent();
    }
}
