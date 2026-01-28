namespace Gck.Application.DTOs;

public class TransactionDto
{
    public int Id { get; set; }
    public int FinancialAccountId { get; set; }
    public string FinancialAccountName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Income" or "Outcome"
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
}

public class CreateTransactionDto
{
    public int FinancialAccountId { get; set; }
    public string Type { get; set; } = string.Empty; // "Income" or "Outcome"
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
}

public class UpdateTransactionDto
{
    public int Id { get; set; }
    public int FinancialAccountId { get; set; }
    public string Type { get; set; } = string.Empty; // "Income" or "Outcome"
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
}

public class TransactionReportDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalOutcome { get; set; }
    public decimal NetAmount { get; set; }
    public List<TransactionDto> Transactions { get; set; } = new();
}
