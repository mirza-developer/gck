using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.Tables.Queries.GetAllTables;

public class GetAllTablesQuery : IRequest<List<TableDto>>
{
}
