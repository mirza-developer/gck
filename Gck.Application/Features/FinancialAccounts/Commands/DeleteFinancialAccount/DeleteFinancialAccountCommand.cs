using MediatR;

namespace Gck.Application.Features.FinancialAccounts.Commands.DeleteFinancialAccount;

public class DeleteFinancialAccountCommand : IRequest<Unit>
{
    public int Id { get; set; }
}
