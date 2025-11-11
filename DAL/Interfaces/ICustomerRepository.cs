using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.IRepositories
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetAllCustomer();
        Customer GetCustomerById(int id);
        Customer? GetByUsername(string username);
        Customer? GetByEmail(string email);
        void Add(Customer customer);
        Task<Customer?> GetByIdAsync(int id);
        Task UpdateAsync(Customer customer);
        List<Customer> GetAllActive();

        Task<Customer?> GetCustomerByIdAsync(int customerId);
    }
}
