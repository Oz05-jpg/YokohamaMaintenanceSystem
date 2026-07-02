using ClosedXML.Excel;
using YokohamaMaintenanceSystem.Interfaces;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Exporters
{
    public class ExcelExporter : IReportExporter
    {
        public byte[] Export(List<MaintenanceRequest> requests)
        {
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

            return bytes;
        }
    }
}
