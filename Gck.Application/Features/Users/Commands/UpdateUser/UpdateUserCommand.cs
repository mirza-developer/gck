using MediatR;

namespace Gck.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<Unit>
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public string? Details { get; set; }
}
