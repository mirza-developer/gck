using MediatR;

namespace Gck.Application.Features.FinancialAccounts.Commands.CreateFinancialAccount;

public class CreateFinancialAccountCommand : IRequest<int>
{
    public string AccountName { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
}
