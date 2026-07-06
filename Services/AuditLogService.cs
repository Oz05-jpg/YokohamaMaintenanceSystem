using YokohamaMaintenanceSystem.Interfaces;

namespace YokohamaMaintenanceSystem.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly List<string> _logs = new List<string>();
        private readonly object _lock = new object();

        public void LogAction(string action)
        {
            lock (_lock)
            {
                _logs.Add(action);
            }
        }

        public List<string> GetLogs()
        {
            lock (_lock)
            {
                // FIX: คืนค่าเป็น List ตัวใหม่ (Copy) 
                // เพื่อให้ Caller เอาไป foreach/Serialize ได้อย่างปลอดภัย 
                // โดยไม่กระทบกับ _logs ตัวจริงที่กำลังถูกเขียนข้าม Thread
                return _logs.ToList();
            }
        }
    }
}