using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IStaffRepository
    {
        //IEnumerable<Staff> GetAll();
        //Staff GetById(int id);
        //void Add(Staff staff);
        //void Update(Staff staff);
        //void Delete(int id);
        Task<Staff?> GetStaffByLoginAsync(string username, string password);
    }
}
