using System.Collections;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Interfaces
{
    public interface IMaintenanceRequestRepository
    {
        object Machines { get; }

        Task<List<MaintenanceRequest>> GetAllAsync();
        Task<MaintenanceRequest?> GetByIdAsync(int id);
        Task AddAsync(MaintenanceRequest request);
        Task UpdateAsync(MaintenanceRequest request);
        Task DeleteAsync(int id);
        object GetByIdAsync();
        Task<IEnumerable> AddAsync(object request);
        Task UpdateAsync(object request);
        Task DeleteAsync(int? id);
    }
}
