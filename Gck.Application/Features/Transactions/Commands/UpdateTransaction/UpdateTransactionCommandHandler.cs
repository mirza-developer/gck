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

        // Validate amount
        if (request.Amount <= 0)
        {
            throw new InvalidOperationException("Transaction amount must be greater than zero.");
        }

        // Validate description
        if (string.IsNullOrWhiteSpace(request.Description))
        {
            throw new InvalidOperationException("Transaction description is required.");
        }

        if (request.Description.Length > 500)
        {
            throw new InvalidOperationException("Transaction description cannot exceed 500 characters.");
        }

        // Validate transaction date
        if (request.TransactionDate > DateTime.Now)
        {
            throw new InvalidOperationException("Transaction date cannot be in the future.");
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
