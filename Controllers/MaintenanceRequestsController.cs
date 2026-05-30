
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

    // GET: MAINTENANCEREQUESTS
    public async Task<IActionResult> Index()
    {
        return View(await _context.MaintenanceRequests.ToListAsync());
    }

    // GET: MAINTENANCEREQUESTS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var maintenancerequest = await _context.MaintenanceRequests
            .FirstOrDefaultAsync(m => m.Id == id);
        if (maintenancerequest == null)
        {
            return NotFound();
        }

        return View(maintenancerequest);
    }

    // GET: MAINTENANCEREQUESTS/Create
    public IActionResult Create()
    {
        ViewBag.MachineId = new SelectList(_context.Machines, "Id", "Name");
        return View();
    }

    // POST: MAINTENANCEREQUESTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Description,Priority,Status,CreatedAt,CompletedAt,MachineId")] MaintenanceRequest maintenancerequest)
    {
        if (ModelState.IsValid)
        {
            _context.Add(maintenancerequest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(maintenancerequest);
    }

    // GET: MAINTENANCEREQUESTS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var maintenancerequest = await _context.MaintenanceRequests.FindAsync(id);
        if (maintenancerequest == null)
        {
            return NotFound();
        }
        ViewBag.MachineId = new SelectList(_context.Machines, "Id", "Name", maintenancerequest.MachineId);
        return View(maintenancerequest);
    }

    // POST: MAINTENANCEREQUESTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,Description,Priority,Status,CreatedAt,CompletedAt,MachineId")] MaintenanceRequest maintenancerequest)
    {
        if (id != maintenancerequest.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(maintenancerequest);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaintenanceRequestExists(maintenancerequest.Id))
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
        return View(maintenancerequest);
    }

    // GET: MAINTENANCEREQUESTS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var maintenancerequest = await _context.MaintenanceRequests
            .FirstOrDefaultAsync(m => m.Id == id);
        if (maintenancerequest == null)
        {
            return NotFound();
        }

        return View(maintenancerequest);
    }

    // POST: MAINTENANCEREQUESTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var maintenancerequest = await _context.MaintenanceRequests.FindAsync(id);
        if (maintenancerequest != null)
        {
            _context.MaintenanceRequests.Remove(maintenancerequest);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MaintenanceRequestExists(int? id)
    {
        return _context.MaintenanceRequests.Any(e => e.Id == id);
    }
}
