using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdVm?>
{
    private readonly GckDbContext _context;

    public GetUserByIdQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<GetUserByIdVm?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Where(u => u.Id == request.Id)
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
