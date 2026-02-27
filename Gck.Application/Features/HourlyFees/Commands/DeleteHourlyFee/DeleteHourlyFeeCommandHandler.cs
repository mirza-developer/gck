using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.HourlyFees.Commands.DeleteHourlyFee;

public class DeleteHourlyFeeCommandHandler : IRequestHandler<DeleteHourlyFeeCommand, Unit>
{
    private readonly GckDbContext _context;

    public DeleteHourlyFeeCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteHourlyFeeCommand request, CancellationToken cancellationToken)
    {
        var hourlyFee = await _context.Fees
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (hourlyFee == null)
        {
            throw new Exception($"HourlyFee with ID {request.Id} not found");
        }

        _context.Fees.Remove(hourlyFee);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
