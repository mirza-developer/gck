using MediatR;

namespace Gck.Application.Features.Customers.Commands.VerifyReferral;

public class VerifyReferralCommand : IRequest<Unit>
{
    public int CustomerId { get; set; }
    public decimal ReferralRewardPercentage { get; set; } = 0;
}
