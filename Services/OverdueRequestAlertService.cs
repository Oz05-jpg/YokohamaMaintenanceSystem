using Microsoft.EntityFrameworkCore;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Interfaces;


namespace YokohamaMaintenanceSystem.Services
{
    public class OverdueRequestAlertService : BackgroundService //BackgroundService คือ class พิเศษที่ .NET รัน ตลอดเวลาที่ app ยังทำงาน — ไม่ต้องรอ request 
    {

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OverdueRequestAlertService> _logger;
        private readonly IConfiguration _config;
        private readonly INotificationStrategy _notifier;
        public OverdueRequestAlertService(
            IServiceScopeFactory scopeFactory, ILogger<OverdueRequestAlertService> logger,
            IConfiguration config,
            INotificationStrategy notifire)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _config = config;
            _notifier = notifire;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var minutes = _config.GetValue<int>("BackgroundServices:OverdueAlertIntervalMinutes");
            var timer = new PeriodicTimer(TimeSpan.FromMinutes(minutes));
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var cutoff = DateTime.UtcNow.AddDays(-3);
                var overdue = await db.MaintenanceRequests
                    .Where(r => r.Status == RequestStatus.Pending &&
                    r.CreatedAt < cutoff)
                    .ToListAsync(stoppingToken);

                if (overdue.Any())
                {
                    _logger.LogWarning("{Count} overdue requests pending", overdue.Count);
                    await _notifier.NotifyAsync($"{overdue.Count} overdue requests pending");
                }
            }
        }
    }
}
