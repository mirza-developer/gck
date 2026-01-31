using Gck.Application.DTOs;
using Gck.Application.Features.Users.Commands.AddUser;
using Gck.Application.Features.Users.Commands.DeleteUser;
using Gck.Application.Features.Users.Commands.UpdateUser;
using Gck.Application.Features.Users.Commands.UpdateUserPassword;
using Gck.Application.Features.Users.Queries.GetAllUsers;
using Gck.Application.Features.Users.Queries.GetUserById;
using Gck.Application.Features.Users.Queries.GetUserByUsername;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gck.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserController> _logger;

    public UserController(IMediator mediator, ILogger<UserController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<GetAllUsersVm>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GetAllUsersVm>>> GetAllUsers()
    {
        var users = await _mediator.Send(new GetAllUsersQuery());
        return Ok(users);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetUserByIdVm), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetUserByIdVm>> GetUserById(string id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery { Id = id });
        if (user == null)
            {
                return NotFound($"User with ID '{id}' not found");
            }
        return Ok(user);
    }

    /// <summary>
    /// Get user by username
    /// </summary>
    [HttpGet("username/{username}")]
    [ProducesResponseType(typeof(GetUserByIdVm), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetUserByIdVm>> GetUserByUsername(string username)
    {
        var user = await _mediator.Send(new GetUserByUsernameQuery { Username = username });
        if (user == null)
            {
                return NotFound($"User with username '{username}' not found");
            }
        return Ok(user);
    }

    /// <summary>
    /// Add new user
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> AddUser([FromBody] AddUserCommand command)
    {
        var userId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUserById), new { id = userId }, userId);
    }

    /// <summary>
    /// Update user
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
        {
        return BadRequest("ID in URL does not match ID in request body");
        }

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Delete user
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        await _mediator.Send(new DeleteUserCommand { Id = id });
        return NoContent();
    }

    /// <summary>
    /// Update user password
    /// </summary>
    [HttpPut("{id}/password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserPassword(string id, [FromBody] UpdateUserPasswordCommand command)
    {
        if (id != command.UserId)
        {
        return BadRequest("ID in URL does not match ID in request body");
        }

        await _mediator.Send(command);
        return NoContent();
    }
}
