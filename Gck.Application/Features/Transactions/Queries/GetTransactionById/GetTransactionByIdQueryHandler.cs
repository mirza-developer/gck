using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Transactions.Queries.GetTransactionById;

public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, TransactionDto?>
{
    private readonly GckDbContext _context;
    private readonly System.Globalization.PersianCalendar _persianCalendar = new();

    public GetTransactionByIdQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<TransactionDto?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions
            .Include(t => t.FinancialAccount)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (transaction == null)
            return null;

        return new TransactionDto
        {
            Id = transaction.Id,
            FinancialAccountId = transaction.FinancialAccountId,
            FinancialAccountName = transaction.FinancialAccount.AccountName,
            Type = transaction.Type,
            Amount = transaction.Amount,
            Description = transaction.Description,
            TransactionDate = ToPersianDateString(transaction.TransactionDate)
        };
    }

    private string ToPersianDateString(DateTime dateTime)
    {
        var year = _persianCalendar.GetYear(dateTime);
        var month = _persianCalendar.GetMonth(dateTime);
        var day = _persianCalendar.GetDayOfMonth(dateTime);
        return $"{year:0000}/{month:00}/{day:00}";
    }
}
