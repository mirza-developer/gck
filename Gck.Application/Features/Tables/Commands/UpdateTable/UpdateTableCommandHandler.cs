using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Tables.Commands.UpdateTable;

public class UpdateTableCommandHandler : IRequestHandler<UpdateTableCommand, Unit>
{
    private readonly GckDbContext _context;

    public UpdateTableCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateTableCommand request, CancellationToken cancellationToken)
    {
        var table = await _context.Tables
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (table == null)
        {
            throw new InvalidOperationException($"Table with ID '{request.Id}' not found.");
        }

        // Only check for name uniqueness if the name has changed
        if (table.Name != request.Name)
        {
            var existingTable = await _context.Tables
                .FirstOrDefaultAsync(t => t.Name == request.Name && t.Id != request.Id, cancellationToken);

            if (existingTable != null)
            {
                throw new InvalidOperationException($"Table with name '{request.Name}' already exists.");
            }
        }

        table.Name = request.Name;
        table.NumberOfControllers = request.NumberOfControllers;
        table.HourlyFeePerController = request.HourlyFeePerController;
        table.LastModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
