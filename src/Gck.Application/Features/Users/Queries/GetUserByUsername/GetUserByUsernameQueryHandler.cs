using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Users.Queries.GetUserByUsername;

public class GetUserByUsernameQueryHandler : IRequestHandler<GetUserByUsernameQuery, GetUserByIdVm?>
{
    private readonly GckDbContext _context;

    public GetUserByUsernameQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<GetUserByIdVm?> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Where(u => u.Username == request.Username)
            .Select(u => new GetUserByIdVm
            {
                Id = u.Id,
                Username = u.Username,
                Name = u.Name,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                IsActive = u.IsActive,
                Details = u.Details
            })
            .FirstOrDefaultAsync(cancellationToken);

        return user;
    }
}
