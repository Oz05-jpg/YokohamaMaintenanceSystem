using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using YokohamaMaintenanceSystem.Interfaces;
using YokohamaMaintenanceSystem.Models;
namespace YokohamaMaintenanceSystem.Exporters
{
    public class PdfExporter : IReportExporter
    {
        public byte[] Export(List<MaintenanceRequest> requests)
        {
            QuestPDF.Settings.License = LicenseType.Community; // Set the license type to Community

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));
                    page.Header()
                        .Text("Maintenance Requests Report")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);
                    page.Content()
                        .Table(table =>
                        {
                            // Define columns
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50); // ID
                                columns.RelativeColumn();    // Title
                                columns.RelativeColumn(); // Status
                                columns.RelativeColumn(); // Machine
                                columns.RelativeColumn(); // CreatedAt
                            });
                            // Header row
                            table.Header(h =>
                            {
                                foreach (var header in new[] { "Id", "Title", "Status", "Machine", "CreatedAt" })
                                {
                                    h.Cell().Text(header);
                                }
                            });

                            // Data rows
                            foreach (var request in requests)
                            {
                                table.Cell().Text(request.Id.ToString());
                                table.Cell().Text(request.Title.ToString());
                                table.Cell().Text(request.Status.ToString());
                                table.Cell().Text(request.Machine?.Name ?? "N/A");
                                table.Cell().Text(request.CreatedAt.ToString("yyyy-MM-dd"));
                            }
                        });
                });
            }).GeneratePdf();
        }
    }
}
