using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.Sessions.Queries.GetDashboardTables;

public class GetDashboardTablesQuery : IRequest<List<DashboardTableDto>>
{
}
