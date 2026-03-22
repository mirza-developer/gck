using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.CreditWithdrawals.Queries.GetAllWithdrawals;

public class GetAllWithdrawalsQueryHandler : IRequestHandler<GetAllWithdrawalsQuery, List<CreditWithdrawalRequestDto>>
{
    private readonly GckDbContext _context;

    public GetAllWithdrawalsQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<List<CreditWithdrawalRequestDto>> Handle(GetAllWithdrawalsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.CreditWithdrawalRequests
            .Include(r => r.Customer)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(r => r.Status == request.Status);
        }

        var results = await query
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
