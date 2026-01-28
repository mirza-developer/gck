using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.FinancialAccounts.Queries.GetAllFinancialAccounts;

public class GetAllFinancialAccountsQueryHandler : IRequestHandler<GetAllFinancialAccountsQuery, List<FinancialAccountDto>>
{
    private readonly GckDbContext _context;

    public GetAllFinancialAccountsQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<List<FinancialAccountDto>> Handle(GetAllFinancialAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = await _context.FinancialAccounts
            .Include(a => a.AccountantReceipts)
            .Include(a => a.Transactions)
            .OrderBy(a => a.AccountName)
            .Select(a => new FinancialAccountDto
            {
                Id = a.Id,
                AccountName = a.AccountName,
                CardNumber = a.CardNumber,
                BankName = a.BankName,
                Balance = a.AccountantReceipts.Sum(r => r.FinalPrice) + 
                          a.Transactions.Where(t => t.Type == "Income").Sum(t => t.Amount) -
                          a.Transactions.Where(t => t.Type == "Outcome").Sum(t => t.Amount)
            })
            .ToListAsync(cancellationToken);

        return accounts;
    }
}
