using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Interfaces
{
    public interface IProfileStaffRepository
    {
        // Lấy thông tin profile theo StaffID
        Task<Staff?> GetProfileByIdAsync(int staffId);

        // Cập nhật thông tin profile
        Task UpdateProfileAsync(Staff staff);
    }
}
