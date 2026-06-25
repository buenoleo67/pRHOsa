namespace pRHosaApp1.Services;

public enum NotificationType
{
    Success,
    Info,
    Warning,
    Error
}

public sealed record AppNotification(Guid Id, string Message, NotificationType Type);

public class NotificationService
{
    private readonly List<AppNotification> notifications = new();

    public event Action? OnChange;

    public IReadOnlyList<AppNotification> Notifications => notifications;

    public void Success(string message, int durationMs = 3200) => Show(message, NotificationType.Success, durationMs);
    public void Info(string message, int durationMs = 2800) => Show(message, NotificationType.Info, durationMs);
    public void Warning(string message, int durationMs = 3600) => Show(message, NotificationType.Warning, durationMs);
    public void Error(string message, int durationMs = 4200) => Show(message, NotificationType.Error, durationMs);

    public void Show(string message, NotificationType type, int durationMs = 3200)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        var notification = new AppNotification(Guid.NewGuid(), message.Trim(), type);
        notifications.Add(notification);
        NotifyStateChanged();
        _ = DismissLaterAsync(notification.Id, durationMs);
    }

    public void Remove(Guid id)
    {
        if (notifications.RemoveAll(n => n.Id == id) > 0)
        {
            NotifyStateChanged();
        }
    }

    private async Task DismissLaterAsync(Guid id, int durationMs)
    {
        await Task.Delay(Math.Max(1200, durationMs));
        Remove(id);
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
