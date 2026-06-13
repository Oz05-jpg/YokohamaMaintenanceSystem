using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {

        private readonly AppDbContext _context;
        public ReportController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _context.MaintenanceRequests.ToListAsync();

            var vm = new ReportViewModel
            {
                PendingCount = requests.Count(r => r.Status == RequestStatus.Pending),
                InProgressCount = requests.Count(r => r.Status == RequestStatus.InProgress),
                CompletedCount = requests.Count(r => r.Status == RequestStatus.Completed)
            };
            return View(vm);
        }

    }
}
