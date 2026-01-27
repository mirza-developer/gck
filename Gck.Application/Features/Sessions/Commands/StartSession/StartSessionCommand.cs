using MediatR;

namespace Gck.Application.Features.Sessions.Commands.StartSession;

public class StartSessionCommand : IRequest<int>
{
    public int TableId { get; set; }
    public int SeatsCount { get; set; }
    public int AnonymousCustomersCount { get; set; } = 0;
    public List<int>? CustomerIds { get; set; }
}
