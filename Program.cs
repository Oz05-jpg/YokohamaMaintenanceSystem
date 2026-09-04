using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using YokohamaMaintenanceSystem.Configuration;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Hubs;
using YokohamaMaintenanceSystem.Interfaces;
using YokohamaMaintenanceSystem.Models;
using YokohamaMaintenanceSystem.Repositories;
using YokohamaMaintenanceSystem.Services;


namespace YokohamaMaintenanceSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllersWithViews()
                .AddJsonOptions(options =>
                 {
                     options.JsonSerializerOptions.ReferenceHandler =
                         System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;//enum IgnoreCycles เพื่อออกจากลูป เจอ object ที่เคย serialize แล้ว → ข้ามไป ไม่วนซ้ำ 
                 });

            // ── Services section
            builder.Services.AddEndpointsApiExplorer();
            // Swagger
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "ใส่แค่ JWT token (ไม่ต้องใส่ Bearer นำหน้า)"
                });
            });

            // JwtSettings
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            builder.Services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey!))
                };
            });

            //OverdueRequestAlertService
            builder.Services.AddHostedService<OverdueRequestAlertService>();

            //SensorSimulationSettings — strongly-typed config + fail-fast validation (TICKET #035)
            builder.Services.AddOptions<SensorSimulationSettings>()
                .Bind(builder.Configuration.GetSection("SensorSimulation"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            //SensorSimulationService IoT
            builder.Services.AddHostedService<SensorSimulationService>();

            //OverdueRequestAlertService
            builder.Services.AddOptions<OverdueAlertSettings>()
               .Bind(builder.Configuration.GetSection("OverdueRequestAlert"))
               .ValidateDataAnnotations()
               .ValidateOnStart();

            //NotificationStrategy
            builder.Services.AddSingleton<INotificationStrategy, SignalRNotificationStrategy>();
            builder.Services.AddSingleton<INotificationStrategy, LogNotificationStrategy>();


            //Cache Dashboard Stats
            builder.Services.AddMemoryCache();

            //SignalR
            builder.Services.AddSignalR();


            // Database
            builder.Services.AddDbContext<AppDbContext>(option =>
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                       .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

            //Health Checks
            builder.Services.AddHealthChecks()
              .AddDbContextCheck<AppDbContext>();


            // Repositories
            builder.Services.AddScoped<IMaintenanceRequestRepository, MaintenanceRequestRepository>();
            builder.Services.AddScoped<IMachineRepository, MachineRepository>();
            builder.Services.AddScoped<ITechnicianRepository, TechnicianRepository>();


            // Identity
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
                options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();
            //Cookie settings
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            // AuditLogservices
            builder.Services.AddSingleton<IAuditLogService, AuditLogService>();

            //MaintenanceNotificationService
            builder.Services.AddSingleton<MaintenanceNotifier>();

            // Static assets
            var app = builder.Build();

            var notifier = app.Services.GetRequiredService<MaintenanceNotifier>();
            var auditLog = app.Services.GetRequiredService<IAuditLogService>();
            notifier.StatusChanged += async (sender, args) =>
            {
                // Log the status change
                auditLog.LogAction($"Status changed: Request {args.RequestId} → {args.NewStatus}");
            };

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error"); //ดัก 500
                app.UseStatusCodePagesWithReExecute("/home/Error/{0}");// status code จริง เช่น /Home/Error/404 ดัก 404/403
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseRouting();//UseExceptionHandler ต้องวาง ก่อน UseRouting() เสมอ
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHealthChecks("/health");

            app.MapStaticAssets();
            app.MapControllers();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapRazorPages();
            app.MapHub<MaintenanceHub>("/maintenancehub");

            // Seed data
            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    db.Database.Migrate();

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

                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    await DbInitializer.SeedAsync(context);
                }
            }
            catch (Exception ex)
            {
                app.Logger.LogWarning(ex, "Seed data ล้มเหลว — ข้ามไปก่อน (DB อาจต่อไม่ได้ในสภาพแวดล้อมนี้)");
            }


            app.Run();
        }
    }
}