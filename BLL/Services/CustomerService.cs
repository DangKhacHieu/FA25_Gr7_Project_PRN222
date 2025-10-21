using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.IServices;
using DAL.Data;
using DAL.Interfaces;
using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class CustomerService : ICustomerService
    {
        /* readonly ICustomerRepository _repository;

        // này dùng để xử lý luồng tìm kiếm 
        private readonly CustomerRepository _repo;
        private readonly PhoneContext _context;

        private List<Customer> _cache;
        public CustomerService(PhoneContext context)
        {
            _context = context;
        }

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }*/

        private readonly ICustomerRepository _repository;
        private readonly CustomerRepository _repo;
        private readonly PhoneContext _context;
        private List<Customer> _cache;

        public CustomerService(ICustomerRepository repository, PhoneContext context)
        {
            _repository = repository;
            _context = context;
        }

        public List<Customer> GetAllCustomers()
        {
            return _repository.GetAllCustomer().ToList();
        }

        public Customer GetCustomerById(int id)
        {
            return _repository.GetCustomerById(id);
        }

        private void LoadCache()
        {
            _cache = _repo.GetAllActive();
        }

        // search chính xác 
        public List<Customer> SearchExact(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return _cache;
            }
            keyword = keyword.ToLower();
            return _cache.Where(c => c.FullName.ToLower() == keyword
            || c.UserName.ToLower() == keyword || c.PhoneNumber == keyword).ToList();

        }

        // Gợi ý autoComplete
        public List<string> Suggest(string term, int maxResults = 5)
        {
            if (_context == null)
            {
                throw new InvalidOperationException("_context is null inside Suggest!");
            }

            if (_context.Customers == null)
            {
                throw new InvalidOperationException("_context.Customers is null! Check DbSet in PhoneContext.");
            }

            term = term ?? "";

            return _context.Customers
                .Where(c => !string.IsNullOrEmpty(c.FullName) && c.FullName.Contains(term))
                .Select(c => c.FullName)
                .Take(maxResults)
                .ToList();

        }
        public void ReloadCache()
        {
            LoadCache();
        }
    }
}
