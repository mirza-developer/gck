using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Transactions.Queries.GetTransactionById;

public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, TransactionDto?>
{
    private readonly GckDbContext _context;

    public GetTransactionByIdQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<TransactionDto?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions
            .Include(t => t.FinancialAccount)
            .Where(t => t.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        return transaction;
    }
}
