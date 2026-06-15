using System.Collections;
using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Interfaces
{
    public interface IMaintenanceRequestRepository
    {
        Task<List<MaintenanceRequest>> GetAllAsync();
        Task<List<MaintenanceRequest>> GetFilteredAsync(string? keyword, RequestStatus? status);
        Task<MaintenanceRequest?> GetByIdAsync(int id);
        Task AddAsync(MaintenanceRequest request);
        Task UpdateAsync(MaintenanceRequest request);
        Task DeleteAsync(int id);
        object GetByIdAsync();
        Task<IEnumerable> AddAsync(object request);
        Task UpdateAsync(object request);
        Task DeleteAsync(int? id);
        void GetFilteredAsync();
    }
}
