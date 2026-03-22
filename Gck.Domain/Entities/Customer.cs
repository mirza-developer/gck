using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gck.Domain.Entities;

[Table("tbl_Customer")]
public class Customer
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(256)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public int BirthYear { get; set; }

    [Required]
    [StringLength(20)]
    public bool IsMale { get; set; } = true;

    public DateTime CreateDate { get; set; }

    public DateTime LastModifiedDate { get; set; }

    // Loyalty Program Fields
    public bool IsLoyal { get; set; } = false;

    public int SessionsRequiredForFree { get; set; } = 0;

    public int PaidSessionsCount { get; set; } = 0;

    // Referral Program Fields
    public int? ReferredByCustomerId { get; set; }

    public bool IsVerifiedByAdmin { get; set; } = true;

    [Column(TypeName = "decimal(18,2)")]
    public decimal ReferralCredit { get; set; } = 0;

    [Column(TypeName = "decimal(5,2)")]
    public decimal ReferralRewardPercentage { get; set; } = 0;

    // Navigation properties
    public virtual ICollection<SessionCustomer> SessionCustomers { get; set; } = new List<SessionCustomer>();

    [ForeignKey(nameof(ReferredByCustomerId))]
    public virtual Customer? ReferredByCustomer { get; set; }

    public virtual ICollection<Customer> ReferredCustomers { get; set; } = new List<Customer>();
}
