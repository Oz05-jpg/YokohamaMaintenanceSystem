using System.ComponentModel.DataAnnotations;

namespace YokohamaMaintenanceSystem.Models
{
    public class Technician
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string FullName { get; set; }

        [Required]
        public required string Specialization { get; set; }

        [MaxLength(20)]
        public required string PhoneNumber { get; set; }
    }
}
