using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class ProfileStaffRepository: IProfileStaffRepository
    {
        private readonly PhoneContext _context;

        public ProfileStaffRepository(PhoneContext context)
        {
            _context = context;
        }

        public async Task<Staff?> GetProfileByIdAsync(int staffId)
        {
            return await _context.Staffs
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StaffID == staffId && s.Status == 1);
        }

        public async Task UpdateProfileAsync(Staff staff)
        {
            _context.Staffs.Update(staff);
            await _context.SaveChangesAsync();
        }
    }
}
