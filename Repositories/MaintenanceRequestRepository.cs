using Microsoft.EntityFrameworkCore;
using System.Collections;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Interfaces;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Repositories
{
    public class MaintenanceRequestRepository : IMaintenanceRequestRepository
    {
        private readonly AppDbContext _context;

        public object Machines => throw new NotImplementedException();

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

        public async Task<List<MaintenanceRequest>> GetFilteredAsync(
        string? keyword, RequestStatus? status)
        {
            var query = _context.MaintenanceRequests.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(r => r.Title.Contains(keyword)
                                  || r.Description.Contains(keyword));

            if (status != null)
                query = query.Where(r => r.Status == status);

            return await query
                .Include(r => r.Machine)
                .Include(r => r.Technician)
                .ToListAsync();
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

        public object GetByIdAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable> AddAsync(object request)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(object request)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public void GetFilteredAsync()
        {
            throw new NotImplementedException();
        }
    }
}
