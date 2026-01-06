using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.FinancialAccounts.Queries.GetFinancialAccountById;

public class GetFinancialAccountByIdQueryHandler : IRequestHandler<GetFinancialAccountByIdQuery, FinancialAccountDto?>
{
    private readonly GckDbContext _context;

    public GetFinancialAccountByIdQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<FinancialAccountDto?> Handle(GetFinancialAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var account = await _context.FinancialAccounts
            .Where(a => a.Id == request.Id)
            .Select(a => new FinancialAccountDto
            {
                Id = a.Id,
                AccountName = a.AccountName,
                CardNumber = a.CardNumber,
                BankName = a.BankName
            })
            .FirstOrDefaultAsync(cancellationToken);

        return account;
    }
}
