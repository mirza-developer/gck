using MediatR;

namespace Gck.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<Unit>
{
    public string Id { get; set; } = string.Empty;
}
