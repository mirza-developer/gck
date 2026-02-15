using Gck.Enums;
using Gck.Models;

namespace Gck.Services;

public interface INotificationService
{
    event Action? OnChange;
    IReadOnlyList<NotificationMessage> Notifications { get; }
    
    void Show(string message, NotificationLevel level = NotificationLevel.Information, NotificationPosition position = NotificationPosition.TopRight);
    void ShowInformation(string message, NotificationPosition position = NotificationPosition.TopRight);
    void ShowSuccess(string message, NotificationPosition position = NotificationPosition.TopRight);
    void ShowWarning(string message, NotificationPosition position = NotificationPosition.TopRight);
    void ShowError(string message, NotificationPosition position = NotificationPosition.TopRight);
    void Remove(Guid id);
}

public class NotificationService : INotificationService
{
    private readonly List<NotificationMessage> _notifications = new();
    
    public event Action? OnChange;
    
    public IReadOnlyList<NotificationMessage> Notifications => _notifications.AsReadOnly();

    public void Show(string message, NotificationLevel level = NotificationLevel.Information, NotificationPosition position = NotificationPosition.TopRight)
    {
        var notification = new NotificationMessage
        {
            Message = message,
            Level = level,
            Position = position
        };
        
        _notifications.Add(notification);
        OnChange?.Invoke();
    }

    public void ShowInformation(string message, NotificationPosition position = NotificationPosition.TopRight)
    {
        Show(message, NotificationLevel.Information, position);
    }

    public void ShowSuccess(string message, NotificationPosition position = NotificationPosition.TopRight)
    {
        Show(message, NotificationLevel.Success, position);
    }

    public void ShowWarning(string message, NotificationPosition position = NotificationPosition.TopRight)
    {
        Show(message, NotificationLevel.Warning, position);
    }

    public void ShowError(string message, NotificationPosition position = NotificationPosition.TopRight)
    {
        Show(message, NotificationLevel.Error, position);
    }

    public void Remove(Guid id)
    {
        var notification = _notifications.FirstOrDefault(n => n.Id == id);
        if (notification != null)
        {
            _notifications.Remove(notification);
            OnChange?.Invoke();
        }
    }
}
