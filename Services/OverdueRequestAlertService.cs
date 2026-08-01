using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using YokohamaMaintenanceSystem.Configuration;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Interfaces;


namespace YokohamaMaintenanceSystem.Services
{
    public class OverdueRequestAlertService : BackgroundService //BackgroundService คือ class พิเศษที่ .NET รัน ตลอดเวลาที่ app ยังทำงาน — ไม่ต้องรอ request 
    {

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OverdueRequestAlertService> _logger;
        private readonly OverdueAlertSettings _setting;
        private readonly IEnumerable<INotificationStrategy> _notifiers;
        public OverdueRequestAlertService(
            IServiceScopeFactory scopeFactory, ILogger<OverdueRequestAlertService> logger,
            IOptions<OverdueAlertSettings> options,
            IEnumerable<INotificationStrategy> notifiers)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _setting = options.Value;
            _notifiers = notifiers;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var minutes = _setting.IntervalMinutes;
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
                    foreach (var notifier in _notifiers)
                    {
                        await notifier.NotifyAsync($"{overdue.Count} overdue requests pending");
                    }
                }
            }
        }
    }
}
