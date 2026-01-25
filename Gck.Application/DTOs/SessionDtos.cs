namespace Gck.Application.DTOs;

public class SessionDto
{
    public int Id { get; set; }
    public int TableId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public decimal FeePerHour { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public bool IsCompleted { get; set; }
    public int AnonymousCustomersCount { get; set; } = 0;
    public decimal? RecommendedPrice { get; set; }
    public decimal? FinalPrice { get; set; }
    public List<CustomerDto> Customers { get; set; } = new();
}

public class StartSessionDto
{
    public int TableId { get; set; }
    public decimal FeePerHour { get; set; }
    public List<int> CustomerIds { get; set; } = new();
}

public class FinishSessionDto
{
    public int SessionId { get; set; }
    public decimal FinalPrice { get; set; }
    public int FinancialAccountId { get; set; }
}

public class DashboardTableDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsOccupied { get; set; }
    public SessionDto? CurrentSession { get; set; }
}
