namespace YokohamaMaintenanceSystem.Models
{
    public class MachinePerformanceViewModel
    {
        public required string MachineName { get; set; }
        public int TotalFixedRequests { get; set; }

        public int TotalFixedInProgress { get; set; }

        public int TotalHighPriority { get; set; }
    }
}
