using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Interfaces
{
    public interface IMaintenanceRequestRepository
    {
        Task<List<MaintenanceRequest>> GetAllAsync();
        Task<List<MaintenanceRequest>> GetFilteredAsync(string? keyword, RequestStatus? status, int pageNumber, int pageSize = 10);
        Task<MaintenanceRequest?> GetByIdAsync(int id);
        Task AddAsync(MaintenanceRequest request);
        Task UpdateAsync(MaintenanceRequest request);
        Task DeleteAsync(int id);
    }
}
