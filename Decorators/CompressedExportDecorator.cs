using System.IO.Compression;
using YokohamaMaintenanceSystem.Interfaces;
using YokohamaMaintenanceSystem.Models;


namespace YokohamaMaintenanceSystem.Decorators
{
    public class CompressedExportDecorator : IReportExporter
    {
        private readonly IReportExporter _inner;

        public CompressedExportDecorator(IReportExporter inner)
        {
            _inner = inner;
        }

        public byte[] Export(List<MaintenanceRequest> requests)
        {
            byte[] rawBytes = _inner.Export(requests);

            using var output = new MemoryStream();
            using (var gzip = new GZipStream(output, CompressionMode.Compress))
            {
                gzip.Write(rawBytes, 0, rawBytes.Length);
            }
            return output.ToArray();
        }
    }
}
