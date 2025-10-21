using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data;
using DAL.IRepositories;
using DAL.Models;

namespace DAL.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly PhoneContext _context;

        public CustomerRepository (PhoneContext context)
        {
            _context = context;
        }

        public IEnumerable<Customer> GetAllCustomer()
        {
            return _context.Customers.ToList();
        }

        public Customer GetCustomerById(int id) 
        {
            return _context.Customers.FirstOrDefault(c => c.CustomerId == id);
        }

        // lấy tất cả khách hàng không bị delted
        public List<Customer> GetAllActive()
        {
            return _context.Customers
                .Where(c => c.Status != -1)
                .ToList();
        }


    }
}
