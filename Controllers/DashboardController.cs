using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            var machinCount = _context.Machines.Count();
            var openRequests = _context.MaintenanceRequests
                               .Count(r => r.Status == "Open");
            var completedRequests = _context.MaintenanceRequests.Count(r => r.Status == "Completed");

            var vm = new DashboardViewModel
            {
                MachinCount = machinCount,
                OpenRequests = openRequests,
                CompletedRequests = completedRequests
            };

            return View(vm);
        }
    }
}
