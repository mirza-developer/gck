using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<GetAllUsersVm>>
{
    private readonly GckDbContext _context;

    public GetAllUsersQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<List<GetAllUsersVm>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .Select(u => new GetAllUsersVm
            {
                Id = u.Id,
                Username = u.Username,
                Name = u.Name,
                IsActive = u.IsActive,
                Details = u.Details
            })
            .ToListAsync(cancellationToken);

        return users;
    }
}
