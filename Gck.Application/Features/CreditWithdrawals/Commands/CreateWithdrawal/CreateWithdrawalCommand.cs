using MediatR;

namespace Gck.Application.Features.CreditWithdrawals.Commands.CreateWithdrawal;

public class CreateWithdrawalCommand : IRequest<CreateWithdrawalResult>
{
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class CreateWithdrawalResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? RequestId { get; set; }
}
