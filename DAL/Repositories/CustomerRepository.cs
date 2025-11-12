using DAL.Data;
using DAL.Interfaces;
using DAL.IRepositories;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Customer? GetByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public Customer? GetByEmail(string email)
        {
            return _context.Customers.FirstOrDefault(c => c.Email == email);
        }

        public void Add(Customer customer)
        {
            throw new NotImplementedException();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }
       

        public void Update(Customer customer)
        {
            _context.Customers.Update(customer);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<Customer?> GetAndUpdateCustomerAsync(Customer customer)
        {
            var existing = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customer.CustomerId);
            if (existing == null) return null;

            existing.FullName = customer.FullName;
            existing.Email = customer.Email;
            existing.PhoneNumber = customer.PhoneNumber;
            existing.Address = customer.Address;
            existing.Sex = customer.Sex;
            existing.DOB = customer.DOB;

            await _context.SaveChangesAsync();
            return existing;
        }
        public bool IsUsernameExist(string username)
        {
            return _context.Customers.Any(c => c.UserName == username);
        }

        public bool IsEmailExist(string email)
        {
            return _context.Customers.Any(c => c.Email == email);
        }

        // ✅ Đăng ký khách hàng mới
        public void RegisterCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }
    }
}
