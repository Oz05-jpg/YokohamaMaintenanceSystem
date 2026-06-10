
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

        public int? TechnicianId { get; set; }
        public Technician? Technician { get; set; }
        //Techniciane? คือการระบุว่า Technician เป็นชนิดที่สามารถเป็น null ได้ ซึ่งหมายความว่า MaintenanceRequest อาจไม่มีการกำหนด Technician ที่รับผิดชอบได้ในบางกรณี เช่น เมื่อคำขอถูกสร้างขึ้นแต่ยังไม่ได้ถูกมอบหมายให้กับช่างเทคนิคใด ๆ หรือเมื่อคำขอถูกยกเลิกและไม่มีช่างเทคนิคที่เกี่ยวข้องอีกต่อไป
    }
}