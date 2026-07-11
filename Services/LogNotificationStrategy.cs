using YokohamaMaintenanceSystem.Interfaces;

namespace YokohamaMaintenanceSystem.Services
{
    public class LogNotificationStrategy : INotificationStrategy
    {

        private readonly ILogger<LogNotificationStrategy> _logger;

        public LogNotificationStrategy(ILogger<LogNotificationStrategy> logger)
        {
            _logger = logger;
        }

        public Task NotifyAsync(string message)
        {
            _logger.LogWarning(message);
            return Task.CompletedTask;
        }
    }

}
