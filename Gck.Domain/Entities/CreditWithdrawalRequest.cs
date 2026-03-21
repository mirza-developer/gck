using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gck.Domain.Entities;

[Table("tbl_CreditWithdrawalRequest")]
public class CreditWithdrawalRequest
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime RequestDate { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

    public DateTime? ProcessedDate { get; set; }

    [StringLength(500)]
    public string Notes { get; set; } = string.Empty;

    public DateTime CreateDate { get; set; }

    // Navigation property
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;
}
