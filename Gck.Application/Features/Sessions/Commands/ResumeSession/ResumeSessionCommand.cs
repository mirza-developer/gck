using MediatR;

namespace Gck.Application.Features.Sessions.Commands.ResumeSession;

public class ResumeSessionCommand : IRequest<Unit>
{
    public int SessionId { get; set; }
}
