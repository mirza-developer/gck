using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<GetUserByIdVm?>
{
    public string Id { get; set; } = string.Empty;
}
