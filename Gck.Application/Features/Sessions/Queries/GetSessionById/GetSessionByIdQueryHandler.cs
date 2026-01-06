using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Sessions.Queries.GetSessionById;

public class GetSessionByIdQueryHandler : IRequestHandler<GetSessionByIdQuery, SessionDto?>
{
    private readonly GckDbContext _context;

    public GetSessionByIdQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<SessionDto?> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
    {
        var session = await _context.Sessions
            .Include(s => s.Table)
            .Include(s => s.SessionCustomers)
            .ThenInclude(sc => sc.Customer)
            .Where(s => s.Id == request.Id)
            .Select(s => new SessionDto
            {
                Id = s.Id,
                TableId = s.TableId,
                TableName = s.Table.Name,
                FeePerHour = s.FeePerHour,
                StartDateTime = s.StartDateTime,
                EndDateTime = s.EndDateTime,
                IsCompleted = s.IsCompleted,
                RecommendedPrice = s.RecommendedPrice,
                FinalPrice = s.FinalPrice,
                Customers = s.SessionCustomers.Select(sc => new CustomerDto
                {
                    Id = sc.Customer.Id,
                    Name = sc.Customer.Name,
                    PhoneNumber = sc.Customer.PhoneNumber,
                    BirthYear = sc.Customer.BirthYear,
                    Gender = sc.Customer.Gender
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return session;
    }
}
