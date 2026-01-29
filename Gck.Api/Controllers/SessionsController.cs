using Gck.Application.DTOs;
using Gck.Application.Features.Sessions.Commands.StartSession;
using Gck.Application.Features.Sessions.Commands.FinishSession;
using Gck.Application.Features.Sessions.Commands.ResumeSession;
using Gck.Application.Features.Sessions.Queries.GetDashboardTables;
using Gck.Application.Features.Sessions.Queries.GetSessionById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gck.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SessionsController> _logger;

    public SessionsController(IMediator mediator, ILogger<SessionsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SessionDto>> GetById(int id)
    {
        var session = await _mediator.Send(new GetSessionByIdQuery { Id = id });
        if (session == null)
            {
                return NotFound($"Session with ID '{id}' not found");
            }
        return Ok(session);
    }

    [HttpPost("start")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> StartSession([FromBody] StartSessionCommand command)
    {
        var sessionId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = sessionId }, sessionId);
    }

    [HttpPost("{id}/finish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FinishSession(int id, [FromBody] FinishSessionCommand command)
    {
        if (id != command.SessionId)
        {
        return BadRequest("ID in URL does not match ID in request body");
        }

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("{id}/resume")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResumeSession(int id)
    {
        await _mediator.Send(new ResumeSessionCommand { SessionId = id });
        return NoContent();
    }
}
