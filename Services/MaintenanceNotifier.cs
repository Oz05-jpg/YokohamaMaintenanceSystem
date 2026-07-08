using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Services
{
    public class MaintenanceNotifier
    {
        public event EventHandler<MaintenanceStatusChangedEventArgs> StatusChanged;
        //เป็นการประกาศ event ที่ชื่อว่า StatusChanged ซึ่งเป็น event handler สำหรับเหตุการณ์ที่เกี่ยวข้องกับการเปลี่ยนแปลงสถานะของคำร้องขอบำรุงรักษา โดยใช้ MaintenanceStatusChangedEventArgs เป็นชนิดของข้อมูลที่ส่งผ่าน event

        public void ChangeStatus(int requestId, RequestStatus newStatus)
        {
            // Notify subscribers about the status change
            StatusChanged?.Invoke(this, new MaintenanceStatusChangedEventArgs   // การเรียกใช้งาน event StatusChanged โดยส่งข้อมูลเกี่ยวกับการเปลี่ยนแปลงสถานะของคำร้องขอบำรุงรักษาไปยังผู้ที่สมัครรับ event
            {
                RequestId = requestId,
                NewStatus = newStatus
            });
        }


    }
}
