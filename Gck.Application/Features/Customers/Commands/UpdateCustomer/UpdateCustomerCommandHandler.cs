using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Unit>
{
    private readonly GckDbContext _context;

    public UpdateCustomerCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID '{request.Id}' not found");
        }

        customer.Name = request.Name;
        customer.PhoneNumber = request.PhoneNumber;
        customer.BirthYear = request.BirthYear;
        customer.IsMale = request.Gender.Equals("Male", StringComparison.OrdinalIgnoreCase);
        customer.IsLoyal = request.IsLoyal;
        customer.SessionsRequiredForFree = request.SessionsRequiredForFree;
        customer.LastModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
