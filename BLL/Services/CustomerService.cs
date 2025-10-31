﻿using BLL.IServices;
using DAL.Data;
using DAL.IRepositories;
using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly PhoneContext _context;

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

        public Customer? GetByLogin(string username, string password)
        {
            return _repository.GetAllCustomer()
                              .FirstOrDefault(c => c.UserName == username && c.Password == password);
        }

        public bool Register(Customer customer, out string message)
        {
            if (string.IsNullOrWhiteSpace(customer.UserName))
            {
                message = "Username is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(customer.Email))
            {
                message = "Email is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(customer.Password))
            {
                message = "Password is required.";
                return false;
            }

            // Kiểm tra trùng username
            var existingUsername = _context.Customers
                .FirstOrDefault(c => c.UserName.ToLower() == customer.UserName.ToLower());

            if (existingUsername != null)
            {
                message = "Username already exists.";
                return false;
            }

            // Kiểm tra trùng email
            var existingEmail = _context.Customers
                .FirstOrDefault(c => c.Email.ToLower() == customer.Email.ToLower());

            if (existingEmail != null)
            {
                message = "Email already exists.";
                return false;
            }

            try
            {
                customer.Status = 1;
                _context.Customers.Add(customer);
                _context.SaveChanges();
                message = "Registration successful!";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Registration failed: {ex.Message}";
                return false;
            }
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            await _repository.UpdateAsync(customer);
        }

        // Các hàm phụ hỗ trợ
        public List<Customer> SearchExact(string keyword)
        {
            var customers = _repository.GetAllCustomer();
            if (string.IsNullOrEmpty(keyword)) return customers.ToList();

            keyword = keyword.ToLower();
            return customers.Where(c =>
                c.FullName.ToLower().Contains(keyword) ||
                c.UserName.ToLower().Contains(keyword) ||
                c.PhoneNumber.Contains(keyword)).ToList();
        }

        public List<string> Suggest(string term, int maxResults = 5)
        {
            term = term ?? "";
            return _context.Customers
                .Where(c => !string.IsNullOrEmpty(c.FullName) && c.FullName.Contains(term))
                .Select(c => c.FullName)
                .Take(maxResults)
                .ToList();
        }

        public void ReloadCache() { }
    }
}
