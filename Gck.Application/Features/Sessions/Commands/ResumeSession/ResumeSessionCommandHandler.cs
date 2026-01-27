using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Sessions.Commands.ResumeSession;

public class ResumeSessionCommandHandler : IRequestHandler<ResumeSessionCommand, Unit>
{
    private readonly GckDbContext _context;

    public ResumeSessionCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(ResumeSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.Sessions
            .Include(s => s.Table)
            .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

        if (session == null)
        {
            throw new InvalidOperationException($"Session with ID '{request.SessionId}' not found.");
        }

        // Reset session state to allow it to continue
        session.EndDateTime = null;
        session.RecommendedPrice = null;
        session.FinalPrice = null;
        session.IsCompleted = false;
        session.LastModifiedDate = DateTime.Now;

        // Mark table as occupied again
        session.Table.IsOccupied = true;
        session.Table.LastModifiedDate = DateTime.Now;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
