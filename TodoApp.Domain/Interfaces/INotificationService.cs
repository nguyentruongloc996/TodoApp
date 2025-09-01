namespace TodoApp.Domain.Interfaces;

public interface INotificationService
{
    void SendNotification(Guid userId, string message);
} 