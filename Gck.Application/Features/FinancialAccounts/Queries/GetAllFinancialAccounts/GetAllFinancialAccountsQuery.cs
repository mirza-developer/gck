using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.FinancialAccounts.Queries.GetAllFinancialAccounts;

public class GetAllFinancialAccountsQuery : IRequest<List<FinancialAccountDto>>
{
}
