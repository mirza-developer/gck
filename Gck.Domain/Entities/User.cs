using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gck.Domain.Entities;

[Table("tbl_User")]
public class User
{
    [Key]
    [StringLength(450)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(256)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(512)]
    public string Name { get; set; } = string.Empty;

    [StringLength(256)]
    public string? Email { get; set; }

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;

    [StringLength(128)]
    public string CreatorIdentityID { get; set; } = string.Empty;

    [StringLength(128)]
    public string? LastModifierIdentityID { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Details { get; set; }

    // Navigation property
    public virtual ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();
}
