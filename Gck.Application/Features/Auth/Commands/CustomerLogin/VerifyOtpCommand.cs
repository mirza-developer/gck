using MediatR;

namespace Gck.Application.Features.Auth.Commands.CustomerLogin;

public class VerifyOtpCommand : IRequest<VerifyOtpResponse>
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
}

public class VerifyOtpResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? PhoneNumber { get; set; }
}
