using Microsoft.EntityFrameworkCore;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Interfaces;

namespace YokohamaMaintenanceSystem.Services
{
    public class SensorSimulationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SensorSimulationService> _logger;
        private readonly IConfiguration _config;
        private readonly IEnumerable<INotificationStrategy> _notifiers;

        public SensorSimulationService(
            IServiceScopeFactory scopeFactory, ILogger<SensorSimulationService> logger,
            IConfiguration config,
            IEnumerable<INotificationStrategy> notifiers)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _config = config;
            _notifiers = notifiers;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) //นี่คือ method ที่จะถูกเรียกเมื่อ service เริ่มทำงาน
        {
            var minutes = _config.GetValue<int>("BackgroundServices:SensorSimulationIntervalMinutes");
            var timer = new PeriodicTimer(TimeSpan.FromMinutes(minutes));
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var runningMachines = await db.Machines
                    .Where(m => m.Status == "Running")
                    .ToListAsync(stoppingToken);
                foreach (var machine in runningMachines)
                {
                    int temperature = Random.Shared.Next(20, 100);
                    // Simulate sensor data
                    if (temperature > 90) //ถ้าอุณหภูมิสูงกว่า 90 องศาเซลเซียส ให้ทำการแจ้งเตือน
                    {
                        _logger.LogWarning("Machine {MachineName} temperature is high: {Temp}", machine.Name, temperature);
                        foreach (var notifier in _notifiers)
                        {
                            await notifier.NotifyAsync($"Machine {machine.Name} temperature is high: {temperature} C - overheating");
                        }
                    }

                }

            }
        }
    }
}
