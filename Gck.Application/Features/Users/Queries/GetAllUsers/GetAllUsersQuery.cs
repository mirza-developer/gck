using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<List<GetAllUsersVm>>
{
}
