using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.HourlyFees.Queries.GetHourlyFeeById;

public class GetHourlyFeeByIdQuery : IRequest<HourlyFeeDto?>
{
    public int Id { get; set; }
}
