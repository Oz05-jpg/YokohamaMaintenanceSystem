using Microsoft.EntityFrameworkCore;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Interfaces;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Repositories
{
    public class MaintenanceRequestRepository : IMaintenanceRequestRepository
    {
        private readonly AppDbContext _context;

        public MaintenanceRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MaintenanceRequest>> GetAllAsync()
        {
            return await _context.MaintenanceRequests.ToListAsync();
        }

        public async Task<MaintenanceRequest?> GetByIdAsync(int id)
        {
            return await _context.MaintenanceRequests.FindAsync(id);
        }

        public async Task AddAsync(MaintenanceRequest request)
        {
            _context.MaintenanceRequests.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MaintenanceRequest request)
        {
            _context.MaintenanceRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var request = await _context.MaintenanceRequests.FindAsync(id);
            if (request != null)
            {
                _context.MaintenanceRequests.Remove(request);
                await _context.SaveChangesAsync();
            }
        }
    }
}
