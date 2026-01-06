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
    public string Gender { get; set; } = "Male";

    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual ICollection<SessionCustomer> SessionCustomers { get; set; } = new List<SessionCustomer>();
}
