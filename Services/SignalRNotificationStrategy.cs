using Microsoft.AspNetCore.SignalR;
using YokohamaMaintenanceSystem.Hubs;
using YokohamaMaintenanceSystem.Interfaces;

namespace YokohamaMaintenanceSystem.Services
{
    public class SignalRNotificationStrategy : INotificationStrategy
    {
        private readonly IHubContext<MaintenanceHub> _hub;

        public SignalRNotificationStrategy(IHubContext<MaintenanceHub> hub)
        {
            _hub = hub;
        }

        public async Task NotifyAsync(string message)
        {
            await _hub.Clients.All.SendAsync("OverdueAlert", message);
        }
    }
}
