namespace Gck.Application.DTOs;

public class FinancialAccountDto
{
    public int Id { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
}

public class CreateFinancialAccountDto
{
    public string AccountName { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
}

public class UpdateFinancialAccountDto
{
    public int Id { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
}
