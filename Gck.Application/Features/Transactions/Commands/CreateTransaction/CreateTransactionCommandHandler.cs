using Gck.Domain.Entities;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Transactions.Commands.CreateTransaction;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, int>
{
    private readonly GckDbContext _context;

    public CreateTransactionCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
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

        var transaction = new Transaction
        {
            FinancialAccountId = request.FinancialAccountId,
            Type = request.Type,
            Amount = request.Amount,
            Description = request.Description,
            TransactionDate = request.TransactionDate,
            CreateDate = DateTime.Now,
            LastModifiedDate = DateTime.Now
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        return transaction.Id;
    }
}
