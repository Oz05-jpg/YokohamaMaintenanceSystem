using System.Reflection.PortableExecutable;

namespace YokohamaMaintenanceSystem.Models
{
    public class MaintenanceRequest
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Priority { get; set; } // Low, Medium, High
        public required string Status { get; set; }   // Pending, In Progress, Completed
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }

        // FK
        public int MachineId { get; set; }
        public Machine? Machine { get; set; }
    }
}