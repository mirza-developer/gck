using MediatR;

namespace Gck.Application.Features.HourlyFees.Commands.UpdateHourlyFee;

public class UpdateHourlyFeeCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public int SeatsCount { get; set; }
    public decimal Fee { get; set; }
}
