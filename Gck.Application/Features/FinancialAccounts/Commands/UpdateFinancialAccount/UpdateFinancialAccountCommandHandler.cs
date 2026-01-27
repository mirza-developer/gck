using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.FinancialAccounts.Commands.UpdateFinancialAccount;

public class UpdateFinancialAccountCommandHandler : IRequestHandler<UpdateFinancialAccountCommand, Unit>
{
    private readonly GckDbContext _context;

    public UpdateFinancialAccountCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateFinancialAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _context.FinancialAccounts
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (account == null)
        {
            throw new InvalidOperationException($"Financial account with ID '{request.Id}' not found");
        }

        account.AccountName = request.AccountName;
        account.CardNumber = request.CardNumber;
        account.BankName = request.BankName;
        account.LastModifiedDate = DateTime.Now;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
