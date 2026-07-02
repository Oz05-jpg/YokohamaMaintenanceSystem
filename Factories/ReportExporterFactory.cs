using YokohamaMaintenanceSystem.Exporters; // ← ExcelExporter,
using YokohamaMaintenanceSystem.Interfaces; // ← IReportExporter

namespace YokohamaMaintenanceSystem.Factories
{

    public class ReportExporterFactory
    {
        public static IReportExporter Create(string format)
        {
            switch (format)
            {
                case "excel": return new ExcelExporter();
                case "csv": return new CsvExporter();
                default: throw new ArgumentException($"Unknown format: {format}");
            }
        }
    }
}
