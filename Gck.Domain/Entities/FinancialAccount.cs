using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gck.Domain.Entities;

[Table("tbl_FinancialAccount")]
public class FinancialAccount
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(256)]
    public string AccountName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string CardNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(256)]
    public string BankName { get; set; } = string.Empty;

    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual ICollection<AccountantReceipt> AccountantReceipts { get; set; } = new List<AccountantReceipt>();
}
