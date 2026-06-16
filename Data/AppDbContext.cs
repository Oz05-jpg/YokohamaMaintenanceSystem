using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Data
{
    //คลาสนี้เป็น DbContext ที่ใช้สำหรับการเชื่อมต่อกับฐานข้อมูลและจัดการข้อมูลของแอปพลิเคชัน
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        //คอนสตรัคเตอร์ที่รับ DbContextOptions เพื่อกำหนดการตั้งค่าการเชื่อมต่อกับฐานข้อมูล
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Models.Machine> Machines { get; set; }//DbSet สำหรับตาราง Machines ในฐานข้อมูล
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }//DbSet สำหรับตาราง MaintenanceRequests ในฐานข้อมูล
        public DbSet<Technician> Technicians { get; set; }//DbSet สำหรับตาราง Technicians ในฐานข้อมูล
    }

    //คลาสนี้ใช้สำหรับการสร้าง DbContext ในระหว่างการออกแบบ (Design Time) เช่น การสร้าง Migration
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=DESKTOP-K9M6MAU\\SQLEXPRESS01;Database=YokohamaMaintenanceDB;Trusted_Connection=True;TrustServerCertificate=True;");
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}