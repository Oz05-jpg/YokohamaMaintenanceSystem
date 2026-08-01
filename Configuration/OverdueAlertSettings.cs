using System.ComponentModel.DataAnnotations;

namespace YokohamaMaintenanceSystem.Configuration
{
    public class OverdueAlertSettings
    {

        [Range(1, 60)]
        public int IntervalMinutes { get; set; }

    }
}
