using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Transactions.Queries.GetAllTransactions;

public class GetAllTransactionsQueryHandler : IRequestHandler<GetAllTransactionsQuery, List<TransactionDto>>
{
    private readonly GckDbContext _context;

    public GetAllTransactionsQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<List<TransactionDto>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _context.Transactions
            .Include(t => t.FinancialAccount)
            .OrderByDescending(t => t.TransactionDate)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                FinancialAccountId = t.FinancialAccountId,
                FinancialAccountName = t.FinancialAccount.AccountName,
                Type = t.Type,
                Amount = t.Amount,
                Description = t.Description,
                TransactionDate = t.TransactionDate
            })
            .ToListAsync(cancellationToken);

        return transactions;
    }
}
