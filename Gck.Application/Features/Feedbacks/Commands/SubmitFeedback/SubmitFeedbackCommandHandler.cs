using Gck.Domain.Entities;
using Gck.Persistence;
using MediatR;

namespace Gck.Application.Features.Feedbacks.Commands.SubmitFeedback;

public class SubmitFeedbackCommandHandler : IRequestHandler<SubmitFeedbackCommand, int>
{
    private readonly GckDbContext _context;

    public SubmitFeedbackCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(SubmitFeedbackCommand request, CancellationToken cancellationToken)
    {
        var feedback = new CustomerFeedback
        {
            CustomerId = request.CustomerId,
            Subject = request.Subject,
            Message = request.Message,
            SubmittedAt = DateTime.Now,
            IsRead = false
        };

        _context.CustomerFeedbacks.Add(feedback);
        await _context.SaveChangesAsync(cancellationToken);

        return feedback.Id;
    }
}
