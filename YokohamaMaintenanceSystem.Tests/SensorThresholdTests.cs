using YokohamaMaintenanceSystem.Services;

namespace YokohamaMaintenanceSystem.Tests
{
    public class SensorThresholdTests
    {
        [Theory]
        [InlineData(70, 60, 90, true)]     // เกิน threshold เฉพาะเครื่อง (และไม่เกิน global 90)
        [InlineData(50, 60, 90, false)]    // ต่ำกว่า
        [InlineData(60, 60, 90, false)]      // เท่ากันพอดี → ? ใช้ false เพราะไม่เกิน thereshold
        [InlineData(70, null, 90, false)]    // ไม่มีค่าเฉพาะเครื่อง → ใช้ global 90 → ?
        [InlineData(95, null, 90, true)]
        public void IsOverheating_ReturnsExpected(int temperature, int? machineThreshold, int globalThreshold, bool expected)
        {
            var result = SensorSimulationService.IsOverheating(temperature, machineThreshold, globalThreshold);
            Assert.Equal(expected, result);
        }
    }
}
