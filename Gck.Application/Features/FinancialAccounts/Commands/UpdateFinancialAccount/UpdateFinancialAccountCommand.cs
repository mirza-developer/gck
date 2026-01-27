using MediatR;

namespace Gck.Application.Features.FinancialAccounts.Commands.UpdateFinancialAccount;

public class UpdateFinancialAccountCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
}
