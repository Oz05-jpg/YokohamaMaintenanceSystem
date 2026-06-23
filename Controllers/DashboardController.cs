using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public DashboardController(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            if (!_cache.TryGetValue("dashboard_stats", out DashboardViewModel vm))
            {
                vm = new DashboardViewModel
                {
                    MachineCount = await _context.Machines.CountAsync(),
                    PendingRequests = await _context.MaintenanceRequests
                        .CountAsync(r => r.Status == RequestStatus.Pending),
                    InProgressRequests = await _context.MaintenanceRequests
                        .CountAsync(r => r.Status == RequestStatus.InProgress),
                    CompletedRequests = await _context.MaintenanceRequests
                        .CountAsync(r => r.Status == RequestStatus.Completed)
                };
                _cache.Set("dashboard_stats", vm, TimeSpan.FromMinutes(5));  // 5 นาที Absolute
            }
            return View(vm);

        }

    }
}