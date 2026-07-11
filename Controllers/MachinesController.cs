
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Interfaces;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Controllers
{
    [Authorize]
    public class MachinesController : Controller
    {

        //Index , Details, Create, Edit - login ธรรมดาเข้าได้
        private readonly AppDbContext _context;
        private readonly IMachineRepository _repo;

        public MachinesController(
            AppDbContext context,
            IMachineRepository repo)
        {
            _context = context;
            _repo = repo;
        }

        // GET: MACHINES
        public async Task<IActionResult> Index()
        {
            return View(await _repo.GetAllAsync());
        }

        // GET: MACHINES/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machine = await _repo.GetByIdAsync(id.Value);

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Location,Status,InstalledDate")] Machine machine)
        {
            if (ModelState.IsValid)
            {
                await _repo.AddAsync(machine);
                return RedirectToAction(nameof(Index));
            }
            return View(machine);
        }

        // GET: MACHINES/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machine = await _repo.GetByIdAsync(id.Value);
            if (machine == null)
            {
                return NotFound();
            }
            return View(machine);
        }

        // POST: MACHINES/Edit
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
                    await _repo.UpdateAsync(machine);
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

        // GET: MACHINES/Delete
        [Authorize(Roles = "Admin")]  // เฉพาะ Delete ต้อง Admin
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machine = await _repo.GetByIdAsync(id.Value);
            if (machine == null)
            {
                return NotFound();
            }

            return View(machine);
        }

        // POST: MACHINES/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var machine = await _repo.GetByIdAsync(id.Value);
            if (machine != null)
                await _repo.DeleteAsync(id.Value);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> MachineExists(int? id)
        {
            return await _context.Machines.AnyAsync(e => e.Id == id);
        }
    }
}
