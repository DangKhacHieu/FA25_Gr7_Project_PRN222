using BLL.IServices;
using DAL.Data;
using DAL.IRepositories;
using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

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
        // THÊM: Phương thức HashPassword (SHA256)
        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // convert byte -> hex
                }
                return builder.ToString();
            }
        }

        // THÊM: Phương thức VerifyPassword (So sánh hash)
        public bool VerifyPassword(string storedHashedPassword, string providedPassword)
        {
            // Hash mật khẩu người dùng cung cấp và so sánh với mật khẩu đã lưu (đã hash)
            var providedHash = HashPassword(providedPassword);
            return storedHashedPassword == providedHash;
        }

        // THÊM: Phương thức Đổi mật khẩu (Change Password)
        public async Task<(bool Success, string Message)> ChangePasswordAsync(int customerId, string oldPassword, string newPassword)
        {
            // 1. Lấy thông tin khách hàng
            var customer = await _repository.GetByIdAsync(customerId);

            if (customer == null || string.IsNullOrEmpty(customer.Password))
            {
                return (false, "Tài khoản không tồn tại hoặc có lỗi dữ liệu.");
            }

            // 2. Xác minh mật khẩu cũ
            if (!VerifyPassword(customer.Password, oldPassword))
            {
                return (false, "Mật khẩu cũ không chính xác.");
            }

            // 3. Hash mật khẩu mới và cập nhật
            customer.Password = HashPassword(newPassword);

            try
            {
                await _repository.UpdateAsync(customer);
                return (true, "Mật khẩu đã được đổi thành công.");
            }
            catch (Exception ex)
            {
                return (false, $"Đổi mật khẩu thất bại: {ex.Message}");
            }
        }
    }
}
