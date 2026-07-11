namespace YokohamaMaintenanceSystem.Interfaces
{
    public interface INotificationStrategy
    {
        Task NotifyAsync(string message);
    }
}
