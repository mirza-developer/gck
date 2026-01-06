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
            .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

        if (session == null)
        {
            throw new InvalidOperationException($"Session with ID '{request.SessionId}' not found.");
        }

        // Reset end time and clear pricing to allow session to continue
        session.EndDateTime = null;
        session.RecommendedPrice = null;
        session.FinalPrice = null;
        session.LastModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
