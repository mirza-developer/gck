using Gck.Domain.Entities;
using Gck.Persistence;
using MediatR;

namespace Gck.Application.Features.FinancialAccounts.Commands.CreateFinancialAccount;

public class CreateFinancialAccountCommandHandler : IRequestHandler<CreateFinancialAccountCommand, int>
{
    private readonly GckDbContext _context;

    public CreateFinancialAccountCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateFinancialAccountCommand request, CancellationToken cancellationToken)
    {
        var account = new FinancialAccount
        {
            AccountName = request.AccountName,
            CardNumber = request.CardNumber,
            BankName = request.BankName,
            CreateDate = DateTime.Now,
            LastModifiedDate = DateTime.Now
        };

        _context.FinancialAccounts.Add(account);
        await _context.SaveChangesAsync(cancellationToken);

        return account.Id;
    }
}
