using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Interfaces
{
    public interface IMaintenanceRequestRepository : IRepository<MaintenanceRequest>
    {
        Task<List<MaintenanceRequest>> GetFilteredAsync(string? keyword, RequestStatus? status, int pageNumber, int pageSize = 10);
    }
}
