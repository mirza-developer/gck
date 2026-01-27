namespace Gck.Application.DTOs;

public class AccountantReceiptDto
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public int FinancialAccountId { get; set; }
    public string FinancialAccountName { get; set; } = string.Empty;
    public decimal RecommendedPrice { get; set; }
    public decimal FinalPrice { get; set; }
    public DateTime ReceiptDateTime { get; set; }
}

public class DashboardAnalyticsDto
{
    public List<DailyReceiptDto> WeeklyReceipts { get; set; } = new();
    public List<MonthlyReceiptDto> YearlyReceipts { get; set; } = new();
}

public class DailyReceiptDto
{
    public string DayName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public bool IsToday { get; set; }
}

public class MonthlyReceiptDto
{
    public string MonthName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public bool IsCurrentMonth { get; set; }
}
