using MediatR;

namespace Gck.Application.Features.Tables.Commands.DeleteTable;

public class DeleteTableCommand : IRequest<Unit>
{
    public int Id { get; set; }
}
