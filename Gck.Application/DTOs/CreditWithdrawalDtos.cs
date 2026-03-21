namespace Gck.Application.DTOs;

public class CreditWithdrawalRequestDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime RequestDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ProcessedDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class CreateCreditWithdrawalRequestDto
{
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class ApproveCreditWithdrawalDto
{
    public int RequestId { get; set; }
    public string Notes { get; set; } = string.Empty;
}
