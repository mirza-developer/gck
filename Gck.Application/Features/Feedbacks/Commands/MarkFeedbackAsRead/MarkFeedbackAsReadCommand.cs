using MediatR;

namespace Gck.Application.Features.Feedbacks.Commands.MarkFeedbackAsRead;

public class MarkFeedbackAsReadCommand : IRequest<bool>
{
    public int Id { get; set; }
}
