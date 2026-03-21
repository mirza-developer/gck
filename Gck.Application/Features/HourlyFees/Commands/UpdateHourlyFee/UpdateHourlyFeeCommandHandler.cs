using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.HourlyFees.Commands.UpdateHourlyFee;

public class UpdateHourlyFeeCommandHandler : IRequestHandler<UpdateHourlyFeeCommand, Unit>
{
    private readonly GckDbContext _context;

    public UpdateHourlyFeeCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateHourlyFeeCommand request, CancellationToken cancellationToken)
    {
        var hourlyFee = await _context.Fees
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (hourlyFee == null)
        {
            throw new InvalidOperationException($"HourlyFee with ID {request.Id} not found");
        }

        hourlyFee.SeatsCount = request.SeatsCount;
        hourlyFee.Fee = request.Fee;
        hourlyFee.LastModifiedDate = DateTime.Now;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
