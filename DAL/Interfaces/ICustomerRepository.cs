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

        void Update(Customer customer);
        void Save();
        Task<Customer?> GetAndUpdateCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task<Customer?> GetCustomerByIdAsync(int id);
        bool IsUsernameExist(string username);
        bool IsEmailExist(string email);

        // ✅ Thêm đăng ký khách hàng
        void RegisterCustomer(Customer customer);



        Task<Customer?> GetCustomerByIdAsyncT(int customerId);

    }
}
