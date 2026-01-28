using MediatR;

namespace Gck.Application.Features.Transactions.Commands.UpdateTransaction;

public class UpdateTransactionCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public int FinancialAccountId { get; set; }
    public string Type { get; set; } = string.Empty; // "Income" or "Outcome"
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
}
