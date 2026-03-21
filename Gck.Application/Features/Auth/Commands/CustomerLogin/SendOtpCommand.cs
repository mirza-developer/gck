using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Gck.Application.Features.Auth.Commands.CustomerLogin;

public class SendOtpCommand : IRequest<SendOtpResponse>
{
    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
}

public class SendOtpResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
