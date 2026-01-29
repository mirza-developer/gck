using Gck.Application.DTOs;
using Gck.Application.Features.Transactions.Commands.CreateTransaction;
using Gck.Application.Features.Transactions.Commands.DeleteTransaction;
using Gck.Application.Features.Transactions.Commands.UpdateTransaction;
using Gck.Application.Features.Transactions.Queries.GetAllTransactions;
using Gck.Application.Features.Transactions.Queries.GetTransactionById;
using Gck.Application.Features.Transactions.Queries.GetTransactionReport;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gck.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<TransactionDto>>> GetAll()
    {
        var transactions = await _mediator.Send(new GetAllTransactionsQuery());
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDto>> GetById(int id)
    {
        var transaction = await _mediator.Send(new GetTransactionByIdQuery { Id = id });
        
        if (transaction == null)
        return NotFound();

        return Ok(transaction);
    }

    [HttpGet("report")]
    public async Task<ActionResult<TransactionReportDto>> GetReport(
        [FromQuery] int? financialAccountId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? type)
    {
        var report = await _mediator.Send(new GetTransactionReportQuery
        {
        FinancialAccountId = financialAccountId,
        StartDate = startDate,
        EndDate = endDate,
        Type = type
        });
        
        return Ok(report);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateTransactionDto dto)
    {
        var command = new CreateTransactionCommand
        {
        FinancialAccountId = dto.FinancialAccountId,
        Type = dto.Type,
        Amount = dto.Amount,
        Description = dto.Description,
        TransactionDate = dto.TransactionDate
        };

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateTransactionDto dto)
    {
        if (id != dto.Id)
        return BadRequest("ID mismatch");

        var command = new UpdateTransactionCommand
        {
        Id = dto.Id,
        FinancialAccountId = dto.FinancialAccountId,
        Type = dto.Type,
        Amount = dto.Amount,
        Description = dto.Description,
        TransactionDate = dto.TransactionDate
        };

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteTransactionCommand { Id = id });
        return NoContent();
    }
}
