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
            IsLoyal = request.IsLoyal,
            SessionsRequiredForFree = request.SessionsRequiredForFree,
            PaidSessionsCount = 0,
            CreateDate = DateTime.Now,
            LastModifiedDate = DateTime.Now
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}
