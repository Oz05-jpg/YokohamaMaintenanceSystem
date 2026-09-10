namespace YokohamaMaintenanceSystem.Models
{
    public class SensorReading
    {
        public int Id { get; set; }

        public int Temperature { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.Now;

        //FK
        public int MachineId { get; set; }
        public required Machine Machine { get; set; }
    }
}
