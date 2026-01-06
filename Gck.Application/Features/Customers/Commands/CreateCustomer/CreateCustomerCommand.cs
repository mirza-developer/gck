using MediatR;

namespace Gck.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int BirthYear { get; set; }
    public string Gender { get; set; } = "Male";
}
