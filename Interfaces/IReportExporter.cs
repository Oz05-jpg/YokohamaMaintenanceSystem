using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Interfaces
{
    public interface IReportExporter
    {
        // method ที่คืน byte[] เพื่อให้ Controller ส่งเป็น file download
        byte[] Export(List<MaintenanceRequest> requests);
    }
}
