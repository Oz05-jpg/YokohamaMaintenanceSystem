using YokohamaMaintenanceSystem.Interfaces;

namespace YokohamaMaintenanceSystem.Services
{

    public class AuditLogService : IAuditLogService
    {
        private readonly List<string> _logs = new List<string>();
        public void LogAction(string action)
        {
            _logs.Add(action);
        }
        public List<string> GetLogs()
        {
            return _logs;
        }
    }
}
