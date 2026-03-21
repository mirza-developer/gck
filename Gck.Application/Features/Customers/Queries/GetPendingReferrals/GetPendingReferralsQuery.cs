using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.Customers.Queries.GetPendingReferrals;

public class GetPendingReferralsQuery : IRequest<List<CustomerDto>>
{
}
