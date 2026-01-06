namespace Gck.Application.DTOs;

public class TableDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int NumberOfControllers { get; set; }
    public decimal HourlyFeePerController { get; set; }
    public bool IsOccupied { get; set; }
}

public class CreateTableDto
{
    public string Name { get; set; } = string.Empty;
    public int NumberOfControllers { get; set; }
    public decimal HourlyFeePerController { get; set; }
}

public class UpdateTableDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int NumberOfControllers { get; set; }
    public decimal HourlyFeePerController { get; set; }
}
