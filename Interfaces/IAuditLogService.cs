namespace YokohamaMaintenanceSystem.Interfaces
{
    public interface IAuditLogService
    {
        void LogAction(string action);
        List<string> GetLogs();
    }
}
