using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Decorators;
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
        public async Task<IActionResult> Export([FromQuery] string format, bool compress = false)
        {
            var requests = await _context.MaintenanceRequests
              .Include(r => r.Machine)
              .ToListAsync();

            try
            {
                var exporter = ReportExporterFactory.Create(format);
                IReportExporter finalExporter = compress ? new CompressedExportDecorator(exporter) : exporter;

                var bytes = finalExporter.Export(requests);

                var contentType = format switch
                {
                    "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "csv" => "text/csv; charset=utf-8",
                    "pdf" => "application/pdf",
                    _ => throw new ArgumentException()

                };

                var fileName = format switch
                {
                    "excel" => "MaintenanceReport.xlsx",
                    "csv" => "MaintenanceReport.csv",
                    "pdf" => "MaintenanceReport.pdf",
                    _ => throw new ArgumentException()
                };

                if (compress)
                {
                    fileName = fileName + ".gz";
                    contentType = "application/gzip";
                }

                return File(bytes, contentType, fileName);

            }
            catch (ArgumentException)
            {
                return BadRequest($"Unknown format: {format}");
            }



        }
    }
}
