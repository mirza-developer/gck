using MediatR;

namespace Gck.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int BirthYear { get; set; }
    public string Gender { get; set; } = string.Empty;
    public bool IsLoyal { get; set; }
    public int SessionsRequiredForFree { get; set; }
    public bool IsVerifiedByAdmin { get; set; } = true;
    public decimal ReferralRewardPercentage { get; set; } = 0;
}
