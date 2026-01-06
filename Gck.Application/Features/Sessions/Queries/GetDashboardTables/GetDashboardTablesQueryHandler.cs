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
            .Include(t => t.Sessions.Where(s => !s.IsCompleted))
            .ThenInclude(s => s.SessionCustomers)
            .ThenInclude(sc => sc.Customer)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

        var dashboardTables = tables.Select(t => new DashboardTableDto
        {
            Id = t.Id,
            Name = t.Name,
            IsOccupied = t.IsOccupied,
            CurrentSession = t.Sessions.FirstOrDefault(s => !s.IsCompleted) != null ? new SessionDto
            {
                Id = t.Sessions.First(s => !s.IsCompleted).Id,
                TableId = t.Id,
                TableName = t.Name,
                FeePerHour = t.Sessions.First(s => !s.IsCompleted).FeePerHour,
                StartDateTime = t.Sessions.First(s => !s.IsCompleted).StartDateTime,
                EndDateTime = t.Sessions.First(s => !s.IsCompleted).EndDateTime,
                IsCompleted = false,
                Customers = t.Sessions.First(s => !s.IsCompleted).SessionCustomers
                    .Select(sc => new CustomerDto
                    {
                        Id = sc.Customer.Id,
                        Name = sc.Customer.Name,
                        PhoneNumber = sc.Customer.PhoneNumber,
                        BirthYear = sc.Customer.BirthYear,
                        Gender = sc.Customer.Gender
                    }).ToList()
            } : null
        }).ToList();

        return dashboardTables;
    }
}
