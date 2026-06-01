using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel
            {
                MachineCount = await _context.Machines.CountAsync(),
                PendingRequests = await _context.MaintenanceRequests
                    .CountAsync(r => r.Status == "Pending"),
                InProgressRequests = await _context.MaintenanceRequests
                    .CountAsync(r => r.Status == "In Progress"),
                CompletedRequests = await _context.MaintenanceRequests
                    .CountAsync(r => r.Status == "Completed")
            };
            return View(vm);
        }
    }
}