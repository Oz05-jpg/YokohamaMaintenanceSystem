using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YokohamaMaintenanceSystem.Interfaces;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Controllers
{
    [Authorize]//เพิ่ม attribute อะไรเพื่อล็อก endpoint ให้เฉพาะ user ที่มี token เท่านั้น?
    [Route("api/maintenance")]
    [ApiController]
    public class MaintenanceApiController : ControllerBase
    {
        private readonly IMaintenanceRequestRepository _repo;

        public MaintenanceApiController(IMaintenanceRequestRepository repo)
        {
            _repo = repo;
        }

        // GET /api/maintenance
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _repo.GetAllAsync();// method ที่ดึงทั้งหมด
            return Ok(requests); //return JSON 200 OK
        }

        // GET/api/maintenance/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var request = await _repo.GetByIdAsync(id);
            if (request == null)
                return NotFound(); //return 404

            return Ok(request);  // return JSON 200 OK
        }

        // PUT/api/maintenance/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest body)  // ✅ [FromBody] UpdateStatusRequest body
        {
            var request = await _repo.GetByIdAsync(id);
            if (request == null)
                return NotFound();//return 404
            request.Status = body.Status;
            await _repo.UpdateAsync(request);
            return NoContent();  // 204 = standard สำหรับ PUT
        }


    }
}
