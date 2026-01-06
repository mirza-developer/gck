using Gck.Application.DTOs;
using Gck.Application.Features.FinancialAccounts.Commands.CreateFinancialAccount;
using Gck.Application.Features.FinancialAccounts.Commands.DeleteFinancialAccount;
using Gck.Application.Features.FinancialAccounts.Commands.UpdateFinancialAccount;
using Gck.Application.Features.FinancialAccounts.Queries.GetAllFinancialAccounts;
using Gck.Application.Features.FinancialAccounts.Queries.GetFinancialAccountById;
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

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FinancialAccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FinancialAccountDto>> GetById(int id)
    {
        try
        {
            var account = await _mediator.Send(new GetFinancialAccountByIdQuery { Id = id });
            if (account == null)
            {
                return NotFound($"Financial account with ID '{id}' not found");
            }
            return Ok(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting financial account by ID: {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the financial account");
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
            return CreatedAtAction(nameof(GetById), new { id = accountId }, accountId);
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

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFinancialAccountCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID in URL does not match ID in request body");
        }

        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating financial account: {Id}", id);
            return StatusCode(500, "An error occurred while updating the financial account");
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _mediator.Send(new DeleteFinancialAccountCommand { Id = id });
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting financial account: {Id}", id);
            return StatusCode(500, "An error occurred while deleting the financial account");
        }
    }
}
