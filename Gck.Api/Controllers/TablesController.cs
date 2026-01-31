using Gck.Application.DTOs;
using Gck.Application.Features.Tables.Commands.CreateTable;
using Gck.Application.Features.Tables.Commands.UpdateTable;
using Gck.Application.Features.Tables.Commands.DeleteTable;
using Gck.Application.Features.Tables.Queries.GetAllTables;
using Gck.Application.Features.Tables.Queries.GetTableById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gck.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TablesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TablesController> _logger;

    public TablesController(IMediator mediator, ILogger<TablesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<TableDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TableDto>>> GetAll()
    {
        var tables = await _mediator.Send(new GetAllTablesQuery());
        return Ok(tables);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TableDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TableDto>> GetById(int id)
    {
        var table = await _mediator.Send(new GetTableByIdQuery { Id = id });
        if (table == null)
            {
                return NotFound($"Table with ID '{id}' not found");
            }
        return Ok(table);
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create([FromBody] CreateTableCommand command)
    {
        var tableId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = tableId }, tableId);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTableCommand command)
    {
        if (id != command.Id)
        {
        return BadRequest("ID in URL does not match ID in request body");
        }

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteTableCommand { Id = id });
        return NoContent();
    }
}
