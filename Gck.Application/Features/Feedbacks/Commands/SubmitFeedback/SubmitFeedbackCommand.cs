using MediatR;

namespace Gck.Application.Features.Feedbacks.Commands.SubmitFeedback;

public class SubmitFeedbackCommand : IRequest<int>
{
    public int CustomerId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
