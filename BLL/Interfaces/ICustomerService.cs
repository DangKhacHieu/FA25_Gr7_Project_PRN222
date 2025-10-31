using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace BLL.IServices
{
    public interface ICustomerService
    {
        List<Customer> GetAllCustomers();
        Customer GetCustomerById(int id);
        bool Register(Customer customer, out string message);
        Customer? GetByLogin(string username, string password);
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task UpdateCustomerAsync(Customer customer);
        List<Customer> SearchExact(string keyword);
        List<string> Suggest(string term, int maxResults = 5);
        void ReloadCache();
    }
}
