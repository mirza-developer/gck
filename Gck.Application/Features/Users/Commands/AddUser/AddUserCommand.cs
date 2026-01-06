using MediatR;

namespace Gck.Application.Features.Users.Commands.AddUser;

public class AddUserCommand : IRequest<string>
{
    public string Username { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Details { get; set; }
}
