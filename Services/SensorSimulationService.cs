using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using YokohamaMaintenanceSystem.Configuration;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Interfaces;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Services
{
    public class SensorSimulationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SensorSimulationService> _logger;
        private readonly SensorSimulationSettings _settings;
        private readonly IEnumerable<INotificationStrategy> _notifiers;

        public SensorSimulationService(
            IServiceScopeFactory scopeFactory, ILogger<SensorSimulationService> logger,
            IOptions<SensorSimulationSettings> options,          // TODO 4: interface ที่ DI ใช้ห่อ strongly-typed settings (ตัวเดียวกับที่ผูกไว้ใน Program.cs)
            IEnumerable<INotificationStrategy> notifiers)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _settings = options.Value;                        // TODO 5: property ที่ดึงค่าจริง (SensorSimulationSettings) ออกมาจาก wrapper
            _notifiers = notifiers;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) //นี่คือ method ที่จะถูกเรียกเมื่อ service เริ่มทำงาน
        {
            var minutes = _settings.IntervalMinutes;
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

                    // เก็บ reading ทุก tick - track ไว้ก่อน ยังไม่ save

                    var reading = new SensorReading
                    {
                        MachineId = machine.Id,
                        Machine = machine,                // satisfy required member
                        Temperature = temperature,
                        RecordedAt = DateTime.Now
                    };
                    await db.SensorReadings.AddAsync(reading);

                    // Simulate sensor data
                    if (temperature > _settings.TemperatureThreshold) //ใช้ threshold จาก config แทนเลข hardcode เดิม (90)
                    {
                        _logger.LogWarning("Machine {MachineName} temperature is high: {Temp}", machine.Name, temperature);
                        foreach (var notifier in _notifiers)
                        {
                            await notifier.NotifyAsync($"Machine {machine.Name} temperature is high: {temperature} C - overheating");
                        }

                        //เช็ค dedup มี request เปิดอยู่ของเครื่องนี้ไหม ถ้าไม่มีให้สร้างใหม่ ถ้ามีอยู่แล้วก็ไม่ต้องทำอะไร
                        bool hasOpenRequest = await db.MaintenanceRequests
                     .AnyAsync(r => r.MachineId == machine.Id && (r.Status == RequestStatus.Pending || r.Status == RequestStatus.InProgress), stoppingToken);
                        if (!hasOpenRequest)
                        {
                            var maintenanceRequest = new MaintenanceRequest
                            {
                                Title = machine.Name,
                                Description = $"Machine {machine.Name} temperature is high: {temperature} C - overheating",
                                Priority = "High",
                                MachineId = machine.Id,
                            };
                            await db.MaintenanceRequests.AddAsync(maintenanceRequest, stoppingToken);
                        }
                    }

                }
                // save changes หลังจาก loop เสร็จ
                await db.SaveChangesAsync();
            }

        }
    }
}
