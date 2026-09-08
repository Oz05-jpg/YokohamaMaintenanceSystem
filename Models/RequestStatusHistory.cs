using YokohamaMaintenanceSystem.Enums;

namespace YokohamaMaintenanceSystem.Models
{
    public class RequestStatusHistory
    {
        public int Id { get; set; }

        // FK
        public int MaintenanceRequestId { get; set; }
        public MaintenanceRequest? MaintenanceRequest { get; set; }

        public RequestStatus OldStatus { get; set; }

        public RequestStatus NewStatus { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.Now;
    }
}
