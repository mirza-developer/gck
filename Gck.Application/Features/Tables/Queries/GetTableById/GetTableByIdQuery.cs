using Gck.Application.DTOs;
using MediatR;

namespace Gck.Application.Features.Tables.Queries.GetTableById;

public class GetTableByIdQuery : IRequest<TableDto?>
{
    public int Id { get; set; }
}
