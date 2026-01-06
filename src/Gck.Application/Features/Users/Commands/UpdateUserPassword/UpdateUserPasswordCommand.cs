using MediatR;

namespace Gck.Application.Features.Users.Commands.UpdateUserPassword;

public class UpdateUserPasswordCommand : IRequest<Unit>
{
    public string UserId { get; set; } = string.Empty;
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
