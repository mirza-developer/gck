using MediatR;

namespace Gck.Application.Features.CreditWithdrawals.Commands.ProcessWithdrawal;

public class ProcessWithdrawalCommand : IRequest<Unit>
{
    public int RequestId { get; set; }
    public string Action { get; set; } = "Approve"; // Approve or Reject
    public string Notes { get; set; } = string.Empty;
}
