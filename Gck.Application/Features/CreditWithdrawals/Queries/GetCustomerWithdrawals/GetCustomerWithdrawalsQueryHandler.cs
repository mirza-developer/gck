using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.CreditWithdrawals.Queries.GetCustomerWithdrawals;

public class GetCustomerWithdrawalsQueryHandler : IRequestHandler<GetCustomerWithdrawalsQuery, List<CreditWithdrawalRequestDto>>
{
    private readonly GckDbContext _context;

    public GetCustomerWithdrawalsQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<List<CreditWithdrawalRequestDto>> Handle(GetCustomerWithdrawalsQuery request, CancellationToken cancellationToken)
    {
        var results = await _context.CreditWithdrawalRequests
            .Include(r => r.Customer)
            .Where(r => r.CustomerId == request.CustomerId)
            .OrderByDescending(r => r.RequestDate)
            .Select(r => new CreditWithdrawalRequestDto
            {
                Id = r.Id,
                CustomerId = r.CustomerId,
                CustomerName = r.Customer.Name,
                CustomerPhone = r.Customer.PhoneNumber,
                Amount = r.Amount,
                RequestDate = r.RequestDate,
                Status = r.Status,
                ProcessedDate = r.ProcessedDate,
                Notes = r.Notes
            })
            .ToListAsync(cancellationToken);

        return results;
    }
}
