using MediatR;

namespace Gck.Application.Features.Tables.Commands.UpdateTable;

public class UpdateTableCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int NumberOfControllers { get; set; }
    public decimal HourlyFeePerController { get; set; }
}
