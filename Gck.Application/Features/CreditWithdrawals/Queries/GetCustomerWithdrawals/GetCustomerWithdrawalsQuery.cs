using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.CreditWithdrawals.Queries.GetCustomerWithdrawals;

public class GetCustomerWithdrawalsQuery : IRequest<List<CreditWithdrawalRequestDto>>
{
    public int CustomerId { get; set; }
}
