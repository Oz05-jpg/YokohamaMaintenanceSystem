
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Controllers
{
    [Authorize]
    public class TechniciansController : Controller
    {
        private readonly AppDbContext _context;

        public TechniciansController(AppDbContext context)
        {
            _context = context;
        }

        // GET: TECHNICIANS
        public async Task<IActionResult> Index()
        {
            return View(await _context.Technicians.ToListAsync());
        }

        // GET: TECHNICIANS/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var technician = await _context.Technicians
                .FirstOrDefaultAsync(m => m.Id == id);
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

        // POST: TECHNICIANS/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost] //คือการระบุว่าเมธอดนี้จะตอบสนองต่อคำขอ HTTP POST เท่านั้น ซึ่งมักใช้สำหรับการส่งข้อมูลจากฟอร์มไปยังเซิร์ฟเวอร์
        [ValidateAntiForgeryToken] //เป็นการป้องกันการโจมตีแบบ Cross-Site Request Forgery (CSRF) โดยการตรวจสอบโทเค็นที่ถูกสร้างขึ้นในฟอร์มและส่งกลับมาเมื่อมีการส่งข้อมูล
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,FullName,Specialization,PhoneNumber")] Technician technician)
        //การใช้ [Bind] เพื่อระบุเฉพาะคุณสมบัติที่ต้องการให้ถูกผูกกับข้อมูลที่ส่งมาจากฟอร์ม ซึ่งช่วยป้องกันการโจมตีแบบ overposting ที่อาจเกิดขึ้นเมื่อมีการส่งข้อมูลที่ไม่ต้องการหรือไม่ปลอดภัยจากฟอร์ม
        {
            if (ModelState.IsValid)//เป็นการตรวจสอบว่าโมเดลที่ถูกส่งมาจากฟอร์มมีความถูกต้องตามกฎที่กำหนดไว้ในโมเดลหรือไม่ เช่น การตรวจสอบว่าฟิลด์ที่จำเป็นถูกกรอกครบถ้วนหรือไม่
            {
                _context.Add(technician);//เป็นการเพิ่มวัตถุ technician ลงในบริบทของฐานข้อมูล ซึ่งจะถูกบันทึกลงในฐานข้อมูลเมื่อเรียกใช้ SaveChangesAsync()
                await _context.SaveChangesAsync();//เป็นการบันทึกการเปลี่ยนแปลงที่เกิดขึ้นในบริบทของฐานข้อมูลลงในฐานข้อมูลจริงๆ โดยใช้คำสั่งแบบอะซิงโครนัสเพื่อไม่ให้บล็อกการทำงานของแอปพลิเคชัน
                return RedirectToAction(nameof(Index));//เป็นการเปลี่ยนเส้นทางไปยังแอคชัน Index หลังจากที่สร้าง technician ใหม่สำเร็จแล้ว
            }
            return View(technician);//ถ้าโมเดลไม่ถูกต้อง จะส่งกลับไปยังมุมมอง Create พร้อมกับข้อมูลที่ผู้ใช้กรอกไว้ เพื่อให้ผู้ใช้สามารถแก้ไขข้อผิดพลาดและส่งข้อมูลใหม่ได้
        }

        // GET: TECHNICIANS/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var technician = await _context.Technicians.FindAsync(id);
            if (technician == null)
            {
                return NotFound();
            }
            return View(technician);
        }

        // POST: TECHNICIANS/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    _context.Update(technician);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TechnicianExists(technician.Id))
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

        // GET: TECHNICIANS/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var technician = await _context.Technicians
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var technician = await _context.Technicians.FindAsync(id);
            if (technician != null)
            {
                _context.Technicians.Remove(technician);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TechnicianExists(int? id)
        {
            return _context.Technicians.Any(e => e.Id == id);
        }
    }
}
