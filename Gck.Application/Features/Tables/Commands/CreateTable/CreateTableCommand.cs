using MediatR;

namespace Gck.Application.Features.Tables.Commands.CreateTable;

public class CreateTableCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public int NumberOfControllers { get; set; }
    public decimal HourlyFeePerController { get; set; }
}
