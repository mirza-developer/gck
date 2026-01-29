using Gck.Application.Features.Auth.Commands.Login;
using Gck.Application.Features.Auth.Commands.CustomerLogin;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gck.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Login with username and password (Admin)
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginCommand command)
    {
        var response = await _mediator.Send(command);
        
        if (!response.Success)
        {
        return Ok(response); // Return 200 with success=false for client handling
        }
        
        return Ok(response);
    }

    /// <summary>
    /// Send OTP to customer phone number
    /// </summary>
    [HttpPost("customer/send-otp")]
    [ProducesResponseType(typeof(SendOtpResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SendOtpResponse>> SendOtp([FromBody] SendOtpCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Verify OTP and login customer
    /// </summary>
    [HttpPost("customer/verify-otp")]
    [ProducesResponseType(typeof(VerifyOtpResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<VerifyOtpResponse>> VerifyOtp([FromBody] VerifyOtpCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }
}
