using Gck.Application.DTOs;
using Gck.Application.Features.Sessions.Queries.GetDashboardTables;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gck.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IMediator mediator, ILogger<DashboardController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("tables")]
    [ProducesResponseType(typeof(List<DashboardTableDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DashboardTableDto>>> GetDashboardTables()
    {
        var tables = await _mediator.Send(new GetDashboardTablesQuery());
        return Ok(tables);
    }
}
