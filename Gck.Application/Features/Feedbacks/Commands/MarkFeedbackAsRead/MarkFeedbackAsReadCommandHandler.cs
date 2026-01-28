using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Feedbacks.Commands.MarkFeedbackAsRead;

public class MarkFeedbackAsReadCommandHandler : IRequestHandler<MarkFeedbackAsReadCommand, bool>
{
    private readonly GckDbContext _context;

    public MarkFeedbackAsReadCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(MarkFeedbackAsReadCommand request, CancellationToken cancellationToken)
    {
        var feedback = await _context.CustomerFeedbacks.FindAsync(new object[] { request.Id }, cancellationToken);
        
        if (feedback == null)
            return false;

        feedback.IsRead = true;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
