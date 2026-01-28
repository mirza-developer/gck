using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gck.Domain.Entities;

[Table("tbl_Transaction")]
public class Transaction
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int FinancialAccountId { get; set; }

    [Required]
    [StringLength(20)]
    public string Type { get; set; } = string.Empty; // "Income" or "Outcome"

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime TransactionDate { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime LastModifiedDate { get; set; }

    // Navigation property
    [ForeignKey(nameof(FinancialAccountId))]
    public virtual FinancialAccount FinancialAccount { get; set; } = null!;
}
