using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Sessions.Queries.GetDashboardTables;

public class GetDashboardTablesQueryHandler : IRequestHandler<GetDashboardTablesQuery, List<DashboardTableDto>>
{
    private readonly GckDbContext _context;

    public GetDashboardTablesQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<List<DashboardTableDto>> Handle(GetDashboardTablesQuery request, CancellationToken cancellationToken)
    {
        var tables = await _context.Tables
            .Include(t => t.Sessions.Where(s => !s.IsCompleted).OrderByDescending(s => s.Id))
            .ThenInclude(s => s.Fee)
            .Include(t => t.Sessions.Where(s => !s.IsCompleted).OrderByDescending(s => s.Id))
            .ThenInclude(s => s.SessionCustomers)
            .ThenInclude(sc => sc.Customer)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

        var dashboardTables = tables.Select(t =>
        {
            // Get the most recent incomplete session for this table
            var currentSession = t.Sessions
                .Where(s => !s.IsCompleted)
                .OrderByDescending(s => s.Id)
                .FirstOrDefault();
                
            return new DashboardTableDto
            {
                Id = t.Id,
                Name = t.Name,
                IsOccupied = t.IsOccupied,
                CurrentSession = currentSession != null ? new SessionDto
                {
                    Id = currentSession.Id,
                    TableId = t.Id,
                    TableName = t.Name,
                    FeePerHour = currentSession.Fee.Fee,
                    StartDateTime = currentSession.StartDateTime,
                    EndDateTime = currentSession.EndDateTime,
                    IsCompleted = false,
                    Customers = currentSession.SessionCustomers
                        .Select(sc => new CustomerDto
                        {
                            Id = sc.Customer.Id,
                            Name = sc.Customer.Name,
                            PhoneNumber = sc.Customer.PhoneNumber,
                            BirthYear = sc.Customer.BirthYear,
                            Gender = sc.Customer.IsMale ? "Male" : "Female"
                        }).ToList()
                } : null
            };
        }).ToList();

        return dashboardTables;
    }
}
