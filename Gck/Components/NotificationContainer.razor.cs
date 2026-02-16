using Gck.Enums;
using Gck.Models;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Components;

public partial class NotificationContainer : ComponentBase, IDisposable
{
    [Inject] private INotificationService NotificationService { get; set; } = default!;
    
    [Parameter]
    public NotificationPosition Position { get; set; } = NotificationPosition.BottomRight;

    protected override void OnInitialized()
    {
        NotificationService.OnChange += StateHasChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var notifications = GetNotificationsForPosition().ToList();
        
        foreach (var notification in notifications)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(10000);
                await InvokeAsync(() => RemoveNotification(notification.Id));
            });
        }
    }

    private IEnumerable<NotificationMessage> GetNotificationsForPosition()
    {
        return NotificationService.Notifications.Where(n => n.Position == Position && n.IsVisible);
    }

    private void RemoveNotification(Guid id)
    {
        NotificationService.Remove(id);
        StateHasChanged();
    }

    private string GetPositionClass()
    {
        return Position switch
        {
            NotificationPosition.TopLeft => "position-top-left",
            NotificationPosition.TopCenter => "position-top-center",
            NotificationPosition.TopRight => "position-top-right",
            NotificationPosition.BottomLeft => "position-bottom-left",
            NotificationPosition.BottomCenter => "position-bottom-center",
            NotificationPosition.BottomRight => "position-bottom-right",
            NotificationPosition.CenterLeft => "position-center-left",
            NotificationPosition.CenterRight => "position-center-right",
            _ => "position-top-right"
        };
    }

    private string GetLevelClass(NotificationLevel level)
    {
        return level switch
        {
            NotificationLevel.Information => "notification-info",
            NotificationLevel.Success => "notification-success",
            NotificationLevel.Warning => "notification-warning",
            NotificationLevel.Error => "notification-error",
            _ => "notification-info"
        };
    }

    private string GetIcon(NotificationLevel level)
    {
        return level switch
        {
            NotificationLevel.Information => "ℹ️",
            NotificationLevel.Success => "✓",
            NotificationLevel.Warning => "⚠️",
            NotificationLevel.Error => "✕",
            _ => "ℹ️"
        };
    }

    public void Dispose()
    {
        NotificationService.OnChange -= StateHasChanged;
    }
}
