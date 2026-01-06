using MediatR;

namespace Gck.Application.Features.Sessions.Commands.FinishSession;

public class FinishSessionCommand : IRequest<Unit>
{
    public int SessionId { get; set; }
    public decimal FinalPrice { get; set; }
    public int FinancialAccountId { get; set; }
}
