using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace BLL.Interfaces
{
    public interface IProfileStaffService
    {
        Task<Staff?> GetProfileByIdAsync(int staffId);
        Task UpdateProfileAsync(Staff staff);
    }
}
