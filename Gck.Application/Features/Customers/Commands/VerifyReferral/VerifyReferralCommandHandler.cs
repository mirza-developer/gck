using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Customers.Commands.VerifyReferral;

public class VerifyReferralCommandHandler : IRequestHandler<VerifyReferralCommand, Unit>
{
    private readonly GckDbContext _context;

    public VerifyReferralCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(VerifyReferralCommand request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID '{request.CustomerId}' not found");
        }

        customer.IsVerifiedByAdmin = true;
        customer.ReferralRewardPercentage = request.ReferralRewardPercentage;
        customer.LastModifiedDate = DateTime.Now;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
