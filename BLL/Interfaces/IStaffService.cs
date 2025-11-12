using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IStaffService
    {
        //IEnumerable<Staff> GetAllStaff();
        //Staff GetStaffById(int id);
        //void AddStaff(Staff staff);
        //void UpdateStaff(Staff staff);
        //void DeleteStaff(int id);
        Task<Staff?> AuthenticateStaffAsync(string username, string password);
    }
}
