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
            .OrderBy(a => a.AccountName)
            .Select(a => new FinancialAccountDto
            {
                Id = a.Id,
                AccountName = a.AccountName,
                CardNumber = a.CardNumber,
                BankName = a.BankName
            })
            .ToListAsync(cancellationToken);

        return accounts;
    }
}
