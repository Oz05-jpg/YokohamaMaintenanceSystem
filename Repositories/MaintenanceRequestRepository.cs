using Microsoft.EntityFrameworkCore;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Interfaces;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Repositories
{
    public class MaintenanceRequestRepository : Repository
        <MaintenanceRequest>, IMaintenanceRequestRepository
    {
        public MaintenanceRequestRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<List<MaintenanceRequest>> GetFilteredAsync(
            string? keyword, RequestStatus? status, int pageNumber, int pageSize = 10)
        {
            var query = _context.MaintenanceRequests.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(r => r.Title.Contains(keyword)
                                  || r.Description.Contains(keyword));

            if (status != null)
                query = query.Where(r => r.Status == status);

            int skip = (pageNumber - 1) * pageSize;

            return await query
                .OrderBy(r => r.Id)
                .Skip(skip)
                .Take(pageSize)
                .Include(r => r.Machine)
                .Include(r => r.Technician)
                .ToListAsync();
        }
    }
}
