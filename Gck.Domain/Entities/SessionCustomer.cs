using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gck.Domain.Entities;

[Table("tbl_SessionCustomer")]
public class SessionCustomer
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int SessionId { get; set; }

    [Required]
    public int CustomerId { get; set; }

    public DateTime CreateDate { get; set; }

    // Navigation properties
    [ForeignKey(nameof(SessionId))]
    public virtual Session Session { get; set; } = null!;

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;
}
