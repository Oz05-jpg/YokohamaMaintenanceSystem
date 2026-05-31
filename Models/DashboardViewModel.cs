namespace YokohamaMaintenanceSystem.Models
{
    public class DashboardViewModel
    {
        public int MachinCount { get; internal set; }
        public int CompletedRequests { get; internal set; }
        public int OpenRequests { get; internal set; }
    }
}
