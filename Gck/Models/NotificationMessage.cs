using Gck.Enums;

namespace Gck.Models;

public class NotificationMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Message { get; set; } = string.Empty;
    public NotificationLevel Level { get; set; } = NotificationLevel.Information;
    public NotificationPosition Position { get; set; } = NotificationPosition.BottomRight;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsVisible { get; set; } = true;
}
