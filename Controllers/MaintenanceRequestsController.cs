using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Models;

[Authorize]
public class MaintenanceRequestsController : Controller
{
    private readonly AppDbContext _context;

    public MaintenanceRequestsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: requests
    public async Task<IActionResult> Index()
    {
        var requests = await _context.MaintenanceRequests
            .Include(r => r.Machine)
            .Include(r => r.Technician)
            .ToListAsync();
        return View(requests);
    }

    // GET: requests/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var requests = await _context.MaintenanceRequests
            .FirstOrDefaultAsync(m => m.Id == id);
        if (requests == null)
        {
            return NotFound();
        }

        return View(requests);
    }

    // GET: requests/Create
    public async Task<IActionResult> CreateAsync()
    {
        // ดึงข้อมูลเครื่องจักรและช่างเทคนิคเพื่อแสดงใน dropdown
        ViewBag.MachineId = new SelectList(
            await _context.Machines.ToListAsync(), "Id", "Name");

        ViewBag.TechnicianId = new SelectList(
            await _context.Technicians.ToListAsync(), "Id", "FullName");

        return View();
    }

    // POST: requests/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Description,Priority,Status,CreatedAt,CompletedAt,MachineId,TechnicianId")] MaintenanceRequest request)
    {
        if (ModelState.IsValid)
        {
            _context.Add(request);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.MachineId = new SelectList(
            await _context.Machines.ToListAsync(), "Id", "Name", request.MachineId);
        ViewBag.TechnicianId = new SelectList(
            await _context.Technicians.ToListAsync(), "Id", "FullName", request.TechnicianId);

        return View(request);
    }

    // GET: requests/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var request = await _context.MaintenanceRequests.FindAsync(id);
        if (request == null)
        {
            return NotFound();
        }
        ViewBag.MachineId = new SelectList(
            await _context.Machines.ToListAsync(), "Id", "Name", request.MachineId);
        ViewBag.TechnicianId = new SelectList(
            await _context.Technicians.ToListAsync(), "Id", "FullName", request.TechnicianId);
        return View(request);
    }

    // POST: requests/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,Description,Priority,Status,CreatedAt,CompletedAt,MachineId,TechnicianId")] MaintenanceRequest request)
    {
        if (id != request.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(request);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaintenanceRequestExists(request.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewBag.MachineId = new SelectList(
            await _context.Machines.ToListAsync(), "Id", "Name", request.MachineId);
        ViewBag.TechnicianId = new SelectList(
            await _context.Technicians.ToListAsync(), "Id", "FullName", request.TechnicianId);

        return View(request);
    }

    // GET: requests/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var requests = await _context.MaintenanceRequests
            .FirstOrDefaultAsync(m => m.Id == id);
        if (requests == null)
        {
            return NotFound();
        }

        return View(requests);
    }

    // POST: requests/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var requests = await _context.MaintenanceRequests.FindAsync(id);
        if (requests != null)
        {
            _context.MaintenanceRequests.Remove(requests);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MaintenanceRequestExists(int? id)
    {
        return _context.MaintenanceRequests.Any(e => e.Id == id);
    }
}
