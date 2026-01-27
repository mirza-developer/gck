using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.HourlyFees.Queries.GetAllHourlyFees;

public class GetAllHourlyFeesQuery : IRequest<List<HourlyFeeDto>>
{
}
