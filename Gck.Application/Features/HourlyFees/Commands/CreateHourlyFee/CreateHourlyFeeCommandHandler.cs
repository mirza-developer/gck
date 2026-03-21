using Gck.Domain.Entities;
using Gck.Persistence;
using MediatR;

namespace Gck.Application.Features.HourlyFees.Commands.CreateHourlyFee;

public class CreateHourlyFeeCommandHandler : IRequestHandler<CreateHourlyFeeCommand, int>
{
    private readonly GckDbContext _context;

    public CreateHourlyFeeCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateHourlyFeeCommand request, CancellationToken cancellationToken)
    {
        var hourlyFee = new HourlyFee
        {
            SeatsCount = request.SeatsCount,
            Fee = request.Fee,
            CreateDate = DateTime.Now,
            LastModifiedDate = DateTime.Now
        };

        _context.Fees.Add(hourlyFee);
        await _context.SaveChangesAsync(cancellationToken);

        return hourlyFee.Id;
    }
}
