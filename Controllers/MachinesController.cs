
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Models;

[Authorize]
public class MachinesController : Controller
{

    //Index , Details, Create, Edit - login ธรรมดาเข้าได้
    private readonly AppDbContext _context;

    public MachinesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: MACHINES
    public async Task<IActionResult> Index()
    {
        return View(await _context.Machines.ToListAsync());
    }

    // GET: MACHINES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var machine = await _context.Machines
            .FirstOrDefaultAsync(m => m.Id == id);
        if (machine == null)
        {
            return NotFound();
        }

        return View(machine);
    }

    // GET: MACHINES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: MACHINES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Location,Status,InstalledDate")] Machine machine)
    {
        if (ModelState.IsValid)
        {
            _context.Add(machine);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(machine);
    }

    // GET: MACHINES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var machine = await _context.Machines.FindAsync(id);
        if (machine == null)
        {
            return NotFound();
        }
        return View(machine);
    }

    // POST: MACHINES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,Location,Status,InstalledDate")] Machine machine)
    {
        if (id != machine.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(machine);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MachineExists(machine.Id))
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
        return View(machine);
    }

    // GET: MACHINES/Delete/5
    [Authorize(Roles = "Admin")]  // เฉพาะ Delete ต้อง Admin
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var machine = await _context.Machines
            .FirstOrDefaultAsync(m => m.Id == id);
        if (machine == null)
        {
            return NotFound();
        }

        return View(machine);
    }

    // POST: MACHINES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var machine = await _context.Machines.FindAsync(id);
        if (machine != null)
        {
            _context.Machines.Remove(machine);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> MachineExists(int? id)
    {
        return await _context.Machines.AnyAsync(e => e.Id == id);
    }
}
