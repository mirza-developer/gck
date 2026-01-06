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
        try
        {
            var tables = await _mediator.Send(new GetAllTablesQuery());
            return Ok(tables);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tables");
            return StatusCode(500, "An error occurred while retrieving tables");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TableDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TableDto>> GetById(int id)
    {
        try
        {
            var table = await _mediator.Send(new GetTableByIdQuery { Id = id });
            if (table == null)
            {
                return NotFound($"Table with ID '{id}' not found");
            }
            return Ok(table);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting table by ID: {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the table");
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create([FromBody] CreateTableCommand command)
    {
        try
        {
            var tableId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = tableId }, tableId);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating table");
            return StatusCode(500, "An error occurred while creating the table");
        }
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
            _logger.LogError(ex, "Error updating table: {Id}", id);
            return StatusCode(500, "An error occurred while updating the table");
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _mediator.Send(new DeleteTableCommand { Id = id });
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting table: {Id}", id);
            return StatusCode(500, "An error occurred while deleting the table");
        }
    }
}
