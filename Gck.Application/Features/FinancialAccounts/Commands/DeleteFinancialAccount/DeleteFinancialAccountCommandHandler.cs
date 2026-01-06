using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.FinancialAccounts.Commands.DeleteFinancialAccount;

public class DeleteFinancialAccountCommandHandler : IRequestHandler<DeleteFinancialAccountCommand, Unit>
{
    private readonly GckDbContext _context;

    public DeleteFinancialAccountCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteFinancialAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _context.FinancialAccounts
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (account == null)
        {
            throw new InvalidOperationException($"Financial account with ID '{request.Id}' not found");
        }

        _context.FinancialAccounts.Remove(account);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
