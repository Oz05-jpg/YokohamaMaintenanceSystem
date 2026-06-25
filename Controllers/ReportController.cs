using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Enums;
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

        public async Task<IActionResult> ExportExcel()
        {
            var requests = await _context.MaintenanceRequests
              .Include(r => r.Machine)
              .ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Report");

            //Header row (row 1)
            ws.Cell(1, 1).Value = "Id";
            ws.Cell(1, 2).Value = "Title";
            ws.Cell(1, 3).Value = "Status";
            ws.Cell(1, 4).Value = "MachineName";
            ws.Cell(1, 5).Value = "CreatedAt";

            // Data rows (เริ่มที่ row 2)
            int row = 2;
            foreach (var r in requests)
            {
                ws.Cell(row, 1).Value = r.Id;
                ws.Cell(row, 2).Value = r.Title;
                ws.Cell(row, 3).Value = r.Status.ToString();   // Status เป็น enum
                ws.Cell(row, 4).Value = r.Machine?.Name ?? "N/A"; // MachineName (nullable)
                ws.Cell(row, 5).Value = r.CreatedAt.ToString("yyyy-MM-dd");
                row++;
            }
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var bytes = stream.ToArray();

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "report.xlsx");
        }


    }
}
