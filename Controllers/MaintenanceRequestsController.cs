using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Hubs;
using YokohamaMaintenanceSystem.Interfaces;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Controllers
{
    [Authorize]
    public class MaintenanceRequestsController : Controller
    {

        private readonly IMaintenanceRequestRepository _repo;
        private readonly AppDbContext _context;
        private readonly ILogger<MaintenanceRequestsController> _logger;
        private readonly IHubContext<MaintenanceHub> _hubContext;
        private EventId exception;

        public MaintenanceRequestsController(
            IMaintenanceRequestRepository repo,
            AppDbContext context,
            ILogger<MaintenanceRequestsController> logger,
            IHubContext<MaintenanceHub> hubContext)
        {
            _repo = repo;
            _context = context;
            _logger = logger;
            _hubContext = hubContext;
        }

        // GET: requests
        public async Task<IActionResult> Index
            (string? keyword,
            RequestStatus? status,
            int pageNumber = 1)
        {
            var requests = await _repo.GetFilteredAsync(keyword, status, pageNumber);


            var viewModels = new PagedRequestViewModel
            {
                Requests = requests,
                CurrentPage = pageNumber,
                HasNextPage = requests.Count == 10, // ถ้าได้ครบ 10 แถว = น่าจะมีหน้าถัดไป
                Keyword = keyword,
                SelectStatus = status
            };

            ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(RequestStatus)));  //เก็บไว้ — ViewModel ไม่มี SelectList สำหรับ dropdown
            return View(viewModels);
        }

        // GET: requests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requests = await _repo.GetByIdAsync(id.Value);

            if (requests == null)
            {
                return NotFound();
            }

            return View(requests);
        }

        // GET: requests/Create
        public async Task<IActionResult> Create()
        {
            // ดึงข้อมูลเครื่องจักรและช่างเทคนิคเพื่อแสดงใน dropdown
            ViewBag.MachineId = new SelectList(
               await _context.Machines.ToListAsync(), "Id", "Name");
            ViewBag.TechnicianId = new SelectList(
                await _context.Technicians.ToListAsync(), "Id", "FullName");
            ViewBag.Status = new SelectList(
                Enum.GetValues<RequestStatus>());
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
                await _repo.AddAsync(request);
                await _hubContext.Clients.All.SendAsync("NewRequest", request.Title);
                //เก็บ log
                _logger.LogInformation("Request created:{Title}", request.Title);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MachineId = new SelectList(
                await _context.Machines.ToListAsync(), "Id", "Name", request.MachineId);
            ViewBag.TechnicianId = new SelectList(
                await _context.Technicians.ToListAsync(), "Id", "FullName", request.TechnicianId);
            ViewBag.Status = new SelectList(
                Enum.GetValues(typeof(RequestStatus)), request.Status);
            return View(request);
        }

        // GET: requests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _repo.GetByIdAsync(id.Value);
            if (request == null)
            {
                return NotFound();
            }
            ViewBag.MachineId = new SelectList(
                await _context.Machines.ToListAsync(), "Id", "Name", request.MachineId);
            ViewBag.TechnicianId = new SelectList(
                await _context.Technicians.ToListAsync(), "Id", "FullName", request.TechnicianId);
            ViewBag.Status = new SelectList(
                Enum.GetValues(typeof(RequestStatus)), request.Status);

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
                    await _repo.UpdateAsync(request);
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
            ViewBag.Status = new SelectList(
                Enum.GetValues<RequestStatus>(), request.Status);
            return View(request);
        }

        // GET: requests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requests = await _repo.GetByIdAsync(id.Value);
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
            var requests = await _repo.GetByIdAsync(id.Value);
            if (requests == null)
            {
                _logger.LogWarning("Delete failed - Request {Id} not found", id);
                return NotFound();
            }
            await _repo.DeleteAsync(id.Value);
            _logger.LogInformation("Request deleted:{Id}", id);
            return RedirectToAction(nameof(Index));
        }

        private bool MaintenanceRequestExists(int? id)
        {
            return _context.MaintenanceRequests.Any(e => e.Id == id);
        }

        // POST: requests/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int? id, string newStatus)
        {
            if (id == null || string.IsNullOrEmpty(newStatus))
            {
                return BadRequest();
            }
            var request = await _repo.GetByIdAsync(id.Value);
            if (request == null)
            {
                return NotFound();
            }
            // อัปเดตสถานะของคำขอ
            if (!Enum.TryParse(newStatus, out RequestStatus status))// แปลงค่า Status ที่เป็น string ให้เป็น enum RequestStatus ถ้าไม่สามารถแปลงได้จะคืนค่า false และ status จะมีค่า default ของ RequestStatus
            {
                return BadRequest();
            }
            request.Status = status; // อัปเดตสถานะของคำขอเป็นค่าที่แปลงได้จาก Status
            try
            {
                await _repo.UpdateAsync(request);
                return RedirectToAction(nameof(Details), new { id = request.Id });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaintenanceRequestExists(request.Id))
                {
                    return NotFound();
                }
                _logger.LogError(exception, "Concurrency error updating Request{Id}", request.Id);
                throw;

            }
        }
    }
}