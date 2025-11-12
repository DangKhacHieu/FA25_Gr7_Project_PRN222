using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;

namespace BLL.Services
{
    public class ProfileStaffService: IProfileStaffService
    {
        private readonly IProfileStaffRepository _profileRepo;

        public ProfileStaffService(IProfileStaffRepository profileRepo)
        {
            _profileRepo = profileRepo;
        }

        public async Task<Staff?> GetProfileByIdAsync(int staffId)
        {
            return await _profileRepo.GetProfileByIdAsync(staffId);
        }

        public async Task UpdateProfileAsync(Staff staff)
        {
            await _profileRepo.UpdateProfileAsync(staff);
        }
    }
}
