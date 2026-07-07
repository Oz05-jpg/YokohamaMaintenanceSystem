using YokohamaMaintenanceSystem.Enums;

namespace YokohamaMaintenanceSystem.Models
{
    public class MaintenanceStatusChangedEventArgs : EventArgs
    {
        public int RequestId { get; set; }
        public RequestStatus NewStatus { get; set; }
    }
}
