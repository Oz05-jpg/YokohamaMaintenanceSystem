using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger; // T = MachineController  อย่าทำ: ILogger<Error>  ← VS ดึง JavaScript Error type มาให้

        public HomeController(AppDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel
            {
                MachineCount = await _context.Machines.CountAsync(),
                PendingRequests = await _context.MaintenanceRequests.CountAsync(r => r.Status == RequestStatus.Pending),
                InProgressRequests = await _context.MaintenanceRequests.CountAsync(r => r.Status == RequestStatus.InProgress),
                CompletedRequests = await _context.MaintenanceRequests.CountAsync(r => r.Status == RequestStatus.Completed),
            };
            return View(vm);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode == 404)
            {
                _logger.LogWarning("404 Not Found: {Path}", HttpContext.Request.Path);
                return View("Error404");
            }

            _logger.LogError("Unhandled exception. RequestId: {Id}",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier);

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}