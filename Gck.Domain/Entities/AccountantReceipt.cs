using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gck.Domain.Entities;

[Table("tbl_AccountantReceipt")]
public class AccountantReceipt
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int SessionId { get; set; }

    [Required]
    public int FinancialAccountId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal RecommendedPrice { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal FinalPrice { get; set; }

    [Required]
    public DateTime ReceiptDateTime { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(SessionId))]
    public virtual Session Session { get; set; } = null!;

    [ForeignKey(nameof(FinancialAccountId))]
    public virtual FinancialAccount FinancialAccount { get; set; } = null!;
}
