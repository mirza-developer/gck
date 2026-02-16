using Gck.Common.Helpers;
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
        var transactionDateTime = PersianDateHelper.FromPersianDateOrNow(request.TransactionDate);
        if (transactionDateTime > DateTime.Now)
        {
            throw new InvalidOperationException("Transaction date cannot be in the future.");
        }

        var transaction = new Transaction
        {
            FinancialAccountId = request.FinancialAccountId,
            Type = request.Type,
            Amount = request.Amount,
            Description = request.Description,
            TransactionDate = transactionDateTime,
            CreateDate = DateTime.Now,
            LastModifiedDate = DateTime.Now
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        return transaction.Id;
    }
}
