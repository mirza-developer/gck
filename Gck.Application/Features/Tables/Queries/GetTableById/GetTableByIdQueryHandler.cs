using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Tables.Queries.GetTableById;

public class GetTableByIdQueryHandler : IRequestHandler<GetTableByIdQuery, TableDto?>
{
    private readonly GckDbContext _context;

    public GetTableByIdQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<TableDto?> Handle(GetTableByIdQuery request, CancellationToken cancellationToken)
    {
        var table = await _context.Tables
            .Where(t => t.Id == request.Id)
            .Select(t => new TableDto
            {
                Id = t.Id,
                Name = t.Name,
                NumberOfControllers = t.NumberOfControllers,
                HourlyFeePerController = t.HourlyFeePerController,
                IsOccupied = t.IsOccupied
            })
            .FirstOrDefaultAsync(cancellationToken);

        return table;
    }
}
