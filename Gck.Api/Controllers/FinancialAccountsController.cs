using Gck.Application.DTOs;
using Gck.Application.Features.FinancialAccounts.Commands.CreateFinancialAccount;
using Gck.Application.Features.FinancialAccounts.Queries.GetAllFinancialAccounts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gck.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FinancialAccountsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FinancialAccountsController> _logger;

    public FinancialAccountsController(IMediator mediator, ILogger<FinancialAccountsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<FinancialAccountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FinancialAccountDto>>> GetAll()
    {
        try
        {
            var accounts = await _mediator.Send(new GetAllFinancialAccountsQuery());
            return Ok(accounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all financial accounts");
            return StatusCode(500, "An error occurred while retrieving financial accounts");
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create([FromBody] CreateFinancialAccountCommand command)
    {
        try
        {
            var accountId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAll), new { id = accountId }, accountId);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating financial account");
            return StatusCode(500, "An error occurred while creating the financial account");
        }
    }
}
