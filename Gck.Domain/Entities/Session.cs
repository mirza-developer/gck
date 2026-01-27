using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gck.Domain.Entities;

[Table("tbl_Session")]
public class Session
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int TableId { get; set; }

    [Required]
    public int FeeId { get; set; }

    [Required]
    public DateTime StartDateTime { get; set; }

    public DateTime? EndDateTime { get; set; }

    public bool IsCompleted { get; set; } = false;

    public bool IsFreeSession { get; set; } = false;

    public int AnonymousCustomersCount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal? RecommendedPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? FinalPrice { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime LastModifiedDate { get; set; }

    // Navigation properties
    [ForeignKey(nameof(TableId))]
    public virtual Table Table { get; set; } = null!;

    [ForeignKey(nameof(FeeId))]
    public virtual HourlyFee Fee { get; set; } = null!;

    public virtual ICollection<SessionCustomer> SessionCustomers { get; set; } = new List<SessionCustomer>();

    public virtual AccountantReceipt? AccountantReceipt { get; set; }
}
