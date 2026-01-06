using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.Users.Queries.GetUserByUsername;

public class GetUserByUsernameQuery : IRequest<GetUserByIdVm?>
{
    public string Username { get; set; } = string.Empty;
}
