using Gck.Application.DTOs;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    private readonly GckDbContext _context;

    public GetCustomerByIdQueryHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers
            .Where(c => c.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        return customer;
    }
}
