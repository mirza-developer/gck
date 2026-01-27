using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gck.Domain.Entities;

[Table("tbl_HourlyFee")]
public class HourlyFee
{
    public int Id { get; set; }

    [Required]
    public int SeatsCount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Fee { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime LastModifiedDate { get; set; }

    // Navigation property
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
