using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Customers.Queries.GetAllCustomers;

public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, List<CustomerDto>>
{
    private readonly GckDbContext _context;

    public GetAllCustomersQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _context.Customers
            .OrderBy(c => c.Name)
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
                PaidSessionsCount = c.PaidSessionsCount
            })
            .ToListAsync(cancellationToken);

        return customers;
    }
}
