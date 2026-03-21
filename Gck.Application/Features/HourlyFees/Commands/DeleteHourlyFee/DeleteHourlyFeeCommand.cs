using MediatR;

namespace Gck.Application.Features.HourlyFees.Commands.DeleteHourlyFee;

public class DeleteHourlyFeeCommand : IRequest<Unit>
{
    public int Id { get; set; }
}
