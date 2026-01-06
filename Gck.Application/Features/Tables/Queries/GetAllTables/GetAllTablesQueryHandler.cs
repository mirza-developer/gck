using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Tables.Queries.GetAllTables;

public class GetAllTablesQueryHandler : IRequestHandler<GetAllTablesQuery, List<TableDto>>
{
    private readonly GckDbContext _context;

    public GetAllTablesQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<List<TableDto>> Handle(GetAllTablesQuery request, CancellationToken cancellationToken)
    {
        var tables = await _context.Tables
            .OrderBy(t => t.Name)
            .Select(t => new TableDto
            {
                Id = t.Id,
                Name = t.Name,
                NumberOfControllers = t.NumberOfControllers,
                HourlyFeePerController = t.HourlyFeePerController,
                IsOccupied = t.IsOccupied
            })
            .ToListAsync(cancellationToken);

        return tables;
    }
}
