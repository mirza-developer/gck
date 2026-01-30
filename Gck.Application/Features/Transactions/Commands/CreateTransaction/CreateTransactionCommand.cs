using MediatR;

namespace Gck.Application.Features.Transactions.Commands.CreateTransaction;

public class CreateTransactionCommand : IRequest<int>
{
    public int FinancialAccountId { get; set; }
    public string Type { get; set; } = string.Empty; // "Income" or "Outcome"
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string TransactionDate { get; set; } = string.Empty;
}
