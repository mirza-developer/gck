using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Transactions.Queries.GetTransactionReport;

public class GetTransactionReportQueryHandler : IRequestHandler<GetTransactionReportQuery, TransactionReportDto>
{
    private readonly GckDbContext _context;

    public GetTransactionReportQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<TransactionReportDto> Handle(GetTransactionReportQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Transactions
            .Include(t => t.FinancialAccount)
            .AsQueryable();

        // Apply filters
        if (request.FinancialAccountId.HasValue)
        {
            query = query.Where(t => t.FinancialAccountId == request.FinancialAccountId.Value);
        }

        if (request.StartDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate <= request.EndDate.Value);
        }

        if (!string.IsNullOrEmpty(request.Type))
        {
            query = query.Where(t => t.Type == request.Type);
        }

        var transactions = await query
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

        var totalIncome = transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
        var totalOutcome = transactions.Where(t => t.Type == "Outcome").Sum(t => t.Amount);

        return new TransactionReportDto
        {
            TotalIncome = totalIncome,
            TotalOutcome = totalOutcome,
            NetAmount = totalIncome - totalOutcome,
            Transactions = transactions
        };
    }
}
