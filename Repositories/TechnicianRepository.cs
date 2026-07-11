using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Interfaces;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Repositories
{
    public class TechnicianRepository : Repository<Technician>, ITechnicianRepository
    {
        public TechnicianRepository(AppDbContext context) : base(context)
        {

        }
    }
}
