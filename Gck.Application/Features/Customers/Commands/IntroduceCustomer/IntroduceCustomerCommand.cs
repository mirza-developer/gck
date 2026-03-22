using MediatR;

namespace Gck.Application.Features.Customers.Commands.IntroduceCustomer;

public class IntroduceCustomerCommand : IRequest<IntroduceCustomerResult>
{
    public int ReferrerCustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int BirthYear { get; set; }
    public string Gender { get; set; } = "Male";
}

public class IntroduceCustomerResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? NewCustomerId { get; set; }
}
