
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Interfaces;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Controllers
{
    [Authorize]
    public class TechniciansController : Controller
    {
        private readonly ITechnicianRepository _repo;

        public TechniciansController(ITechnicianRepository repo)
        {
            _repo = repo;
        }

        // GET: TECHNICIANS
        public async Task<IActionResult> Index()
        {
            return View(await _repo.GetAllAsync());
        }

        // GET: TECHNICIANS/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var technician = await _repo.GetByIdAsync(id.Value);
            if (technician == null)
            {
                return NotFound();
            }

            return View(technician);
        }

        // GET: TECHNICIANS/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,FullName,Specialization,PhoneNumber")] Technician technician)
        {
            if (ModelState.IsValid)
            {
                await _repo.AddAsync(technician);
                return RedirectToAction(nameof(Index));
            }
            return View(technician);
        }

        // GET: TECHNICIANS/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var technician = await _repo.GetByIdAsync(id.Value);
            if (technician == null)
            {
                return NotFound();
            }
            return View(technician);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,FullName,Specialization,PhoneNumber")] Technician technician)
        {
            if (id != technician.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repo.UpdateAsync(technician);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _repo.ExistsAsync(technician.Id))
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
            return View(technician);
        }

        // GET: TECHNICIANS/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var technician = await _repo.GetByIdAsync(id.Value);
            if (technician == null)
            {
                return NotFound();
            }

            return View(technician);


        }

        // POST: TECHNICIANS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var technician = await _repo.DeleteAsync(id.Value);
            if (technician == null)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
