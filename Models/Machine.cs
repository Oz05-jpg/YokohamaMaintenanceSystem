namespace YokohamaMaintenanceSystem.Models
{
    public class Machine
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Location { get; set; }
        public required string Status { get; set; } // Running, Stopped, Under Maintenance
        public DateTime InstalledDate { get; set; }
        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = [];
    }
}