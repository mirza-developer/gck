using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.FinancialAccounts.Queries.GetFinancialAccountById;

public class GetFinancialAccountByIdQuery : IRequest<FinancialAccountDto?>
{
    public int Id { get; set; }
}
