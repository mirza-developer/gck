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
        try
        {
            var response = await _mediator.Send(command);
            
            if (!response.Success)
            {
                return Ok(response); // Return 200 with success=false for client handling
            }
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new LoginResponse 
            { 
                Success = false, 
                Message = "خطا در فرآیند ورود به سیستم" 
            });
        }
    }

    /// <summary>
    /// Send OTP to customer phone number
    /// </summary>
    [HttpPost("customer/send-otp")]
    [ProducesResponseType(typeof(SendOtpResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SendOtpResponse>> SendOtp([FromBody] SendOtpCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending OTP");
            return StatusCode(500, new SendOtpResponse
            {
                Success = false,
                Message = "خطا در ارسال کد تایید"
            });
        }
    }

    /// <summary>
    /// Verify OTP and login customer
    /// </summary>
    [HttpPost("customer/verify-otp")]
    [ProducesResponseType(typeof(VerifyOtpResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<VerifyOtpResponse>> VerifyOtp([FromBody] VerifyOtpCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying OTP");
            return StatusCode(500, new VerifyOtpResponse
            {
                Success = false,
                Message = "خطا در تایید کد"
            });
        }
    }
}
