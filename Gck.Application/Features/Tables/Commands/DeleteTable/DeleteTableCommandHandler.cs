using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Tables.Commands.DeleteTable;

public class DeleteTableCommandHandler : IRequestHandler<DeleteTableCommand, Unit>
{
    private readonly GckDbContext _context;

    public DeleteTableCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteTableCommand request, CancellationToken cancellationToken)
    {
        var table = await _context.Tables
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (table == null)
        {
            throw new InvalidOperationException($"Table with ID '{request.Id}' not found.");
        }

        _context.Tables.Remove(table);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
