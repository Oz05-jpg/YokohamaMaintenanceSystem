using System.ComponentModel.DataAnnotations;

namespace YokohamaMaintenanceSystem.Configuration
{
    public class SensorSimulationSettings
    {
        [Range(1, 60)]
        public int IntervalMinutes { get; set; }

        [Range(1, 200)]
        public int TemperatureThreshold { get; set; }
    }
}
