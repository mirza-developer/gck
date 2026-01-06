using Gck.Domain.Entities;
using Gck.Persistence;
using MediatR;

namespace Gck.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, int>
{
    private readonly GckDbContext _context;

    public CreateCustomerCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            BirthYear = request.BirthYear,
            IsMale = request.Gender.Equals("Male", StringComparison.OrdinalIgnoreCase),
            CreateDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}
