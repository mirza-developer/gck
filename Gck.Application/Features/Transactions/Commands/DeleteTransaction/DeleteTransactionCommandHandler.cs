using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Transactions.Commands.DeleteTransaction;

public class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, Unit>
{
    private readonly GckDbContext _context;

    public DeleteTransactionCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (transaction == null)
        {
            throw new InvalidOperationException($"Transaction with ID '{request.Id}' not found.");
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
