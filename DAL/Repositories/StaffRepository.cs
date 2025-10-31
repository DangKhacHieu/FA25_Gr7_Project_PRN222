using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class StaffRepository
    {
        private readonly PhoneContext _context;

        public StaffRepository(PhoneContext context)
        {
            _context = context;
        }

        // Lấy danh sách staff có Status = 0 (chưa xóa)
        public async Task<IEnumerable<Staff>> GetAllAsync()
        {
            return await _context.Staffs
                .Where(s => s.Status == 0)
                .ToListAsync();
        }

        // Lấy staff theo ID
        public async Task<Staff?> GetByIdAsync(int id)
        {
            return await _context.Staffs
                .FirstOrDefaultAsync(s => s.StaffID == id && s.Status == 0);
        }

        // Thêm mới
        public async Task AddAsync(Staff staff)
        {
            staff.Status = 0; // mặc định chưa xóa
            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();
        }

        // Cập nhật
        public async Task UpdateAsync(Staff staff)
        {
            _context.Staffs.Update(staff);
            await _context.SaveChangesAsync();
        }

        // Xóa mềm (update Status = 1)
        public async Task SoftDeleteAsync(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff != null)
            {
                staff.Status = 1;
                _context.Staffs.Update(staff);
                await _context.SaveChangesAsync();
            }
        }
    }
}