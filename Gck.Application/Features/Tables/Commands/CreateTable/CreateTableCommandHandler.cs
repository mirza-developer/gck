using Gck.Domain.Entities;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Tables.Commands.CreateTable;

public class CreateTableCommandHandler : IRequestHandler<CreateTableCommand, int>
{
    private readonly GckDbContext _context;

    public CreateTableCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTableCommand request, CancellationToken cancellationToken)
    {
        var existingTable = await _context.Tables
            .FirstOrDefaultAsync(t => t.Name == request.Name, cancellationToken);

        if (existingTable != null)
        {
            throw new InvalidOperationException($"Table with name '{request.Name}' already exists.");
        }

        var table = new Table
        {
            Name = request.Name,
            IsOccupied = false,
            CreateDate = DateTime.Now,
            LastModifiedDate = DateTime.Now
        };

        _context.Tables.Add(table);
        await _context.SaveChangesAsync(cancellationToken);

        return table.Id;
    }
}
