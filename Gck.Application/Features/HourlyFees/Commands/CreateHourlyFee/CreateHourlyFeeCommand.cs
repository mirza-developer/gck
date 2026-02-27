using MediatR;

namespace Gck.Application.Features.HourlyFees.Commands.CreateHourlyFee;

public class CreateHourlyFeeCommand : IRequest<int>
{
    public int SeatsCount { get; set; }
    public decimal Fee { get; set; }
}
