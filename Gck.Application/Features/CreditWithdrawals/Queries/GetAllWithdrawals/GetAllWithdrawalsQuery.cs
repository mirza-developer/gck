using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.CreditWithdrawals.Queries.GetAllWithdrawals;

public class GetAllWithdrawalsQuery : IRequest<List<CreditWithdrawalRequestDto>>
{
    public string? Status { get; set; } // null = all, "Pending", "Approved", "Rejected"
}
