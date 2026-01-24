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

    // Navigation property
    public virtual ICollection<SessionCustomer> SessionCustomers { get; set; } = new List<SessionCustomer>();
}
