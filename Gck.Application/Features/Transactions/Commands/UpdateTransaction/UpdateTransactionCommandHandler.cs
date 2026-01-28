using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Transactions.Commands.UpdateTransaction;

public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, Unit>
{
    private readonly GckDbContext _context;

    public UpdateTransactionCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (transaction == null)
        {
            throw new InvalidOperationException($"Transaction with ID '{request.Id}' not found.");
        }

        // Validate financial account exists
        var accountExists = await _context.FinancialAccounts
            .AnyAsync(f => f.Id == request.FinancialAccountId, cancellationToken);

        if (!accountExists)
        {
            throw new InvalidOperationException($"Financial account with ID '{request.FinancialAccountId}' not found.");
        }

        // Validate type
        if (request.Type != "Income" && request.Type != "Outcome")
        {
            throw new InvalidOperationException("Transaction type must be either 'Income' or 'Outcome'.");
        }

        transaction.FinancialAccountId = request.FinancialAccountId;
        transaction.Type = request.Type;
        transaction.Amount = request.Amount;
        transaction.Description = request.Description;
        transaction.TransactionDate = request.TransactionDate;
        transaction.LastModifiedDate = DateTime.Now;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
