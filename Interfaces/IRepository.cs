namespace YokohamaMaintenanceSystem.Interfaces
{

    //Generic Repository คือการสร้าง interface สำหรับการทำงานกับฐานข้อมูลที่สามารถใช้ได้กับหลายๆ entity โดยไม่จำเป็นต้องสร้าง repository แยกสำหรับแต่ละ entity ซึ่งช่วยลดความซ้ำซ้อนของโค้ดและทำให้การจัดการข้อมูลมีความยืดหยุ่นมากขึ้น
    public interface IRepository<T>
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T?> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
