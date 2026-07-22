namespace YokohamaMaintenanceSystem.Models
{
    public class TechnicianPerformanceViewModel
    {
        public required string FullName { get; set; }
        public int TotalAssigned { get; set; }
        public int TotalCompleted { get; set; }
    }
}
