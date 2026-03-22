using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Customers.Queries.GetPendingReferrals;

public class GetPendingReferralsQueryHandler : IRequestHandler<GetPendingReferralsQuery, List<CustomerDto>>
{
    private readonly GckDbContext _context;

    public GetPendingReferralsQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerDto>> Handle(GetPendingReferralsQuery request, CancellationToken cancellationToken)
    {
        var customers = await _context.Customers
            .Include(c => c.ReferredByCustomer)
            .Where(c => !c.IsVerifiedByAdmin && c.ReferredByCustomerId.HasValue)
            .OrderByDescending(c => c.CreateDate)
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                PhoneNumber = c.PhoneNumber,
                BirthYear = c.BirthYear,
                Gender = c.IsMale ? "Male" : "Female",
                SessionCount = c.SessionCustomers.Count,
                IsLoyal = c.IsLoyal,
                SessionsRequiredForFree = c.SessionsRequiredForFree,
                PaidSessionsCount = c.PaidSessionsCount,
                ReferredByCustomerId = c.ReferredByCustomerId,
                ReferredByCustomerName = c.ReferredByCustomer != null ? c.ReferredByCustomer.Name : null,
                IsVerifiedByAdmin = c.IsVerifiedByAdmin,
                ReferralCredit = c.ReferralCredit,
                ReferralRewardPercentage = c.ReferralRewardPercentage
            })
            .ToListAsync(cancellationToken);

        return customers;
    }
}
