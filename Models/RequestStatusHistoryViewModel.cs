namespace YokohamaMaintenanceSystem.Models
{
    public class RequestStatusHistoryViewModel
    {
        public required MaintenanceRequest Request { get; set; }

        public List<RequestStatusHistory> Histories { get; set; } = new();

    }
}
