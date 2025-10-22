using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class StaffService
    {
        private readonly StaffRepository _repository;

        public StaffService(StaffRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Staff>> GetAllAsync() => _repository.GetAllAsync();
        public Task<Staff?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
        public Task AddAsync(Staff staff) => _repository.AddAsync(staff);
        public Task UpdateAsync(Staff staff) => _repository.UpdateAsync(staff);
        public Task SoftDeleteAsync(int id) => _repository.SoftDeleteAsync(id);
    }
}
