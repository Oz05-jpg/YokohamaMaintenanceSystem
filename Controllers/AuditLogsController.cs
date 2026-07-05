using Microsoft.AspNetCore.Mvc;
using YokohamaMaintenanceSystem.Interfaces;

namespace YokohamaMaintenanceSystem.Controllers
{
    [ApiController]
    [Route("api/audit-logs")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;
        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public IActionResult GetLogs()
        {
            return Ok(_auditLogService.GetLogs());
        }
    }
}
