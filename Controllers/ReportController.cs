using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Factories;
using YokohamaMaintenanceSystem.Interfaces;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IMaintenanceRequestRepository _repo;
        public ReportController(AppDbContext context,
            IMaintenanceRequestRepository repo)
        {
            _context = context;
            _repo = repo;
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

        // ExportExcel
        public async Task<IActionResult> Export([FromQuery] string format)
        {
            var requests = await _context.MaintenanceRequests
              .Include(r => r.Machine)
              .ToListAsync();

            try
            {
                var exporter = ReportExporterFactory.Create(format);
                var bytes = exporter.Export(requests);

                var contentType = format == "excel"
    ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    : "text/csv; charset=utf-8";  // ← เพิ่ม charset

                var fileName = format == "excel" ? "report.xlsx" : "report.csv";

                return File(bytes, contentType, fileName);

            }
            catch (ArgumentException)
            {
                return BadRequest($"Unknown format: {format}");
            }



        }
    }
}
