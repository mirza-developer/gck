using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.HourlyFees.Queries.GetHourlyFeeById;

public class GetHourlyFeeByIdQueryHandler : IRequestHandler<GetHourlyFeeByIdQuery, HourlyFeeDto?>
{
    private readonly GckDbContext _context;

    public GetHourlyFeeByIdQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<HourlyFeeDto?> Handle(GetHourlyFeeByIdQuery request, CancellationToken cancellationToken)
    {
        var fee = await _context.Fees
            .Where(f => f.Id == request.Id)
            .Select(f => new HourlyFeeDto
            {
                Id = f.Id,
                SeatsCount = f.SeatsCount,
                Fee = f.Fee
            })
            .FirstOrDefaultAsync(cancellationToken);

        return fee;
    }
}
