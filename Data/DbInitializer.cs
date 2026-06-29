using Microsoft.EntityFrameworkCore;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Models;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Step 1: ล้างทุกอย่างก่อนเสมอ
        if (context.Database.IsRelational())
        {
            // SQL Server — ใช้ raw SQL (bypass EF tracking)
            await context.Database.ExecuteSqlRawAsync("DELETE FROM MaintenanceRequests");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Machines");
        }
        else
        {
            // InMemory (test environment) — ใช้ EF แทน
            context.MaintenanceRequests.RemoveRange(context.MaintenanceRequests);
            context.Machines.RemoveRange(context.Machines);
            await context.SaveChangesAsync();
        }

        // Step 2: Seed Machines ใหม่
        context.Machines.AddRange(
            new Machine { Name = "เครื่องรีดยาง A1", Location = "โซน A", Status = "Running", InstalledDate = new DateTime(2020, 1, 1) },
            new Machine { Name = "เครื่องอัดยาง B2", Location = "โซน B", Status = "Running", InstalledDate = new DateTime(2020, 3, 1) },
            new Machine { Name = "เครื่องตัด C3",    Location = "โซน C", Status = "Running", InstalledDate = new DateTime(2021, 1, 1) },
            new Machine { Name = "เครื่องเชื่อม D4", Location = "โซน D", Status = "Stopped", InstalledDate = new DateTime(2021, 6, 1) },
            new Machine { Name = "เครื่องพ่นสี E5",  Location = "โซน E", Status = "Under Maintenance", InstalledDate = new DateTime(2022, 1, 1) }
        );
        await context.SaveChangesAsync();

        // Step 3: ดึง machineIds จาก DB fresh
        var machineIds = await context.Machines.Select(m => m.Id).ToListAsync();

        if (machineIds.Count < 5)
            throw new InvalidOperationException($"ต้องการ 5 machines แต่มีแค่ {machineIds.Count}");

        // Step 4: Seed MaintenanceRequests
        context.MaintenanceRequests.AddRange(
            new MaintenanceRequest { Title = "ซ่อมมอเตอร์",            Description = "มอเตอร์ร้อนผิดปกติ",      Priority = "High",   Status = RequestStatus.Pending,    CreatedAt = DateTime.Now.AddDays(-14), MachineId = machineIds[0] },
            new MaintenanceRequest { Title = "เปลี่ยนสายพาน",           Description = "สายพานขาด",              Priority = "High",   Status = RequestStatus.InProgress,  CreatedAt = DateTime.Now.AddDays(-13), MachineId = machineIds[1] },
            new MaintenanceRequest { Title = "ตรวจสอบระบบไฮดรอลิก",    Description = "น้ำมันรั่ว",             Priority = "Medium", Status = RequestStatus.Pending,    CreatedAt = DateTime.Now.AddDays(-12), MachineId = machineIds[2] },
            new MaintenanceRequest { Title = "เปลี่ยนฟิลเตอร์",         Description = "ฟิลเตอร์อุดตัน",        Priority = "Low",    Status = RequestStatus.Completed,  CreatedAt = DateTime.Now.AddDays(-11), CompletedAt = DateTime.Now.AddDays(-9),  MachineId = machineIds[3] },
            new MaintenanceRequest { Title = "ซ่อมระบบระบายความร้อน",   Description = "อุณหภูมิสูงเกิน",       Priority = "High",   Status = RequestStatus.InProgress,  CreatedAt = DateTime.Now.AddDays(-10), MachineId = machineIds[4] },
            new MaintenanceRequest { Title = "ตรวจเช็คประจำปี",          Description = "PM ประจำปี",             Priority = "Low",    Status = RequestStatus.Completed,  CreatedAt = DateTime.Now.AddDays(-9),  CompletedAt = DateTime.Now.AddDays(-7),  MachineId = machineIds[0] },
            new MaintenanceRequest { Title = "เปลี่ยนแบริ่ง",           Description = "เสียงดังผิดปกติ",       Priority = "Medium", Status = RequestStatus.Pending,    CreatedAt = DateTime.Now.AddDays(-8),  MachineId = machineIds[1] },
            new MaintenanceRequest { Title = "ซ่อมระบบไฟฟ้า",           Description = "ไฟฟ้าขัดข้อง",          Priority = "High",   Status = RequestStatus.Pending,    CreatedAt = DateTime.Now.AddDays(-7),  MachineId = machineIds[2] },
            new MaintenanceRequest { Title = "ปรับแต่งความดัน",          Description = "ความดันต่ำกว่าเกณฑ์",   Priority = "Medium", Status = RequestStatus.InProgress,  CreatedAt = DateTime.Now.AddDays(-6),  MachineId = machineIds[3] },
            new MaintenanceRequest { Title = "เปลี่ยนซีล",              Description = "ซีลรั่วน้ำมัน",         Priority = "Medium", Status = RequestStatus.Pending,    CreatedAt = DateTime.Now.AddDays(-5),  MachineId = machineIds[4] },
            new MaintenanceRequest { Title = "ซ่อมปั๊มน้ำ",             Description = "ปั๊มไม่ทำงาน",          Priority = "High",   Status = RequestStatus.Pending,    CreatedAt = DateTime.Now.AddDays(-4),  MachineId = machineIds[0] },
            new MaintenanceRequest { Title = "ตรวจสอบเซ็นเซอร์",        Description = "เซ็นเซอร์อ่านค่าผิด",  Priority = "Low",    Status = RequestStatus.Completed,  CreatedAt = DateTime.Now.AddDays(-3),  CompletedAt = DateTime.Now.AddDays(-1),  MachineId = machineIds[1] },
            new MaintenanceRequest { Title = "เปลี่ยนหัวฉีด",           Description = "หัวฉีดอุดตัน",          Priority = "Medium", Status = RequestStatus.InProgress,  CreatedAt = DateTime.Now.AddDays(-2),  MachineId = machineIds[2] },
            new MaintenanceRequest { Title = "ซ่อมระบบลม",              Description = "ลมรั่ว",                Priority = "Low",    Status = RequestStatus.Pending,    CreatedAt = DateTime.Now.AddDays(-1),  MachineId = machineIds[3] },
            new MaintenanceRequest { Title = "ตรวจเช็คสายไฟ",           Description = "สายไฟชำรุด",            Priority = "High",   Status = RequestStatus.Completed,  CreatedAt = DateTime.Now,              CompletedAt = DateTime.Now,              MachineId = machineIds[4] }
        );
        await context.SaveChangesAsync();
    }
}
