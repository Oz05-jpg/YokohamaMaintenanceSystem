using System.Text;
using YokohamaMaintenanceSystem.Interfaces;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Exporters
{
    public class CsvExporter : IReportExporter
    {
        public byte[] Export(List<MaintenanceRequest> requests)
        {
            var sb = new StringBuilder();

            // Header
            sb.AppendLine("Id,Title,Status,MachineName,CreatedAt");

            // Data rows
            foreach (var r in requests)
            {
                sb.AppendLine($"{r.Id},{r.Title},{r.Status},{r.Machine?.Name ?? "N/A"},{r.CreatedAt}");
            }

            return new UTF8Encoding(true).GetBytes(sb.ToString());
        }
    }
}
