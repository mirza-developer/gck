using MediatR;

namespace Gck.Application.Features.Sessions.Commands.StartSession;

public class StartSessionCommand : IRequest<int>
{
    public int TableId { get; set; }
    public decimal FeePerHour { get; set; }
    public List<int> CustomerIds { get; set; } = new();
}
