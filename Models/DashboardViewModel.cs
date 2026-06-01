namespace YokohamaMaintenanceSystem.Models
{
    public class DashboardViewModel
    {
        public int MachineCount { get; set; }
        public int PendingRequests { get; set; }
        public int InProgressRequests { get; set; }
        public int CompletedRequests { get; set; }
    }
}