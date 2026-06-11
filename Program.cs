using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDbContext>(option =>
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
                options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapRazorPages();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Roles
                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));

                // Admin user
                var adminUser = await userManager.FindByEmailAsync("admin@yokohama.com");
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser { UserName = "admin@yokohama.com", Email = "admin@yokohama.com" };
                    await userManager.CreateAsync(adminUser, "Admin@1234");
                }
                await userManager.AddToRoleAsync(adminUser, "Admin");

                // Machines
                if (!db.Machines.Any())
                {
                    db.Machines.AddRange(
                        new Machine { Name = "CNC-001", Location = "Hall A", Status = "Running", InstalledDate = DateTime.Now },
                        new Machine { Name = "Lathe-002", Location = "Hall B", Status = "Running", InstalledDate = DateTime.Now },
                        new Machine { Name = "Press-003", Location = "Hall C", Status = "Stopped", InstalledDate = DateTime.Now }
                    );
                    await db.SaveChangesAsync();
                }

                // Technicians
                if (!db.Technicians.Any())
                {
                    db.Technicians.AddRange(
                        new Technician { FullName = "Somchai Jaidee", Specialization = "Mechanical", PhoneNumber = "081-111-1111" },
                        new Technician { FullName = "Wichai Saengdee", Specialization = "Electrical", PhoneNumber = "082-222-2222" }
                    );
                    await db.SaveChangesAsync();
                }

                // Maintenance Requests
                if (!db.MaintenanceRequests.Any())
                {
                    var machine1 = db.Machines.First();
                    var tech1 = db.Technicians.First();
                    db.MaintenanceRequests.AddRange(
                        new MaintenanceRequest { Title = "ซ่อมสายพาน", Description = "สายพานขาด", Priority = "High", Status = RequestStatus.Pending, MachineId = machine1.Id },
                        new MaintenanceRequest { Title = "เปลี่ยนน้ำมัน", Description = "น้ำมันหมด", Priority = "Medium", Status = RequestStatus.InProgress, MachineId = machine1.Id, TechnicianId = tech1.Id },
                        new MaintenanceRequest { Title = "ตรวจสอบระบบ", Description = "PM ประจำเดือน", Priority = "Low", Status = RequestStatus.Completed, MachineId = machine1.Id, TechnicianId = tech1.Id, CompletedAt = DateTime.Now }
                    );
                    await db.SaveChangesAsync();
                }
            }

            app.Run();
        }
    }
}