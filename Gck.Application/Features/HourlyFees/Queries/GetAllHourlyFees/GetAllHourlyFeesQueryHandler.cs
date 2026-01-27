using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.HourlyFees.Queries.GetAllHourlyFees;

public class GetAllHourlyFeesQueryHandler : IRequestHandler<GetAllHourlyFeesQuery, List<HourlyFeeDto>>
{
    private readonly GckDbContext _context;

    public GetAllHourlyFeesQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<List<HourlyFeeDto>> Handle(GetAllHourlyFeesQuery request, CancellationToken cancellationToken)
    {
        var fees = await _context.Fees
            .OrderBy(f => f.SeatsCount)
            .Select(f => new HourlyFeeDto
            {
                Id = f.Id,
                SeatsCount = f.SeatsCount,
                Fee = f.Fee
            })
            .ToListAsync(cancellationToken);

        return fees;
    }
}
