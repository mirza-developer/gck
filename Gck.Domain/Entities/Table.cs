using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gck.Domain.Entities;

[Table("tbl_Table")]
public class Table
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int NumberOfControllers { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal HourlyFeePerController { get; set; }

    public bool IsOccupied { get; set; } = false;

    public DateTime CreateDate { get; set; }

    public DateTime LastModifiedDate { get; set; }

    // Navigation property
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
