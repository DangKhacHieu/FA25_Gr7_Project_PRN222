using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly PhoneContext _context;
        public ProductRepository(PhoneContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Where(p => p.IsDelete == 0)
                .ToListAsync();


        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.ProductID == id && p.IsDelete == 0);
        }

        public async Task<IEnumerable<Product>> SearchByNameAsync(string keyword)
        {
            return await _context.Products
                .Where(p => p.IsDelete == 0 && p.ProductName.StartsWith(keyword))
                .ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Products.AnyAsync(e => e.ProductID == id);
        }


        public IQueryable<Product> GetQueryableProducts()
        {
            // Trả về IQueryable để xây dựng truy vấn động
            return _context.Products.Where(p => p.IsDelete == 0);
        }

        public async Task<List<string>> GetDistinctBrandsAsync()
        {
            return await _context.Products
                .Where(p => p.IsDelete == 0 && !string.IsNullOrEmpty(p.Brand))
                .Select(p => p.Brand!)
                .Distinct()
                .OrderBy(b => b)
                .ToListAsync();
        }

        public async Task<List<string>> GetDistinctRamsAsync()
        {
            return await _context.Products
                .Where(p => p.IsDelete == 0 && !string.IsNullOrEmpty(p.Ram))
                .Select(p => p.Ram!)
                .Distinct()
                .OrderBy(r => r)
                .ToListAsync();
        }

        public async Task<List<string>> GetDistinctRomsAsync()
        {
            return await _context.Products
                .Where(p => p.IsDelete == 0 && !string.IsNullOrEmpty(p.Rom))
                .Select(p => p.Rom!)
                .Distinct()
                .OrderBy(r => r)
                .ToListAsync();
        }

        public async Task IncreaseProductQuantityAsync(int productId, int quantityToAdd)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product != null)
            {
                // Sử dụng GetValueOrDefault để xử lý trường hợp Quantity_Product là null
                int currentQuantity = product.Quantity_Product.GetValueOrDefault();

                product.Quantity_Product = currentQuantity + quantityToAdd;

                // Chỉ đánh dấu thuộc tính Quantity_Product là Modified
                _context.Entry(product).Property(p => p.Quantity_Product).IsModified = true;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetPagedProductsAsync(int pageIndex, int pageSize)
        {
            return await _context.Products
                .Where(p => p.IsDelete == 0)
                .OrderBy(p => p.ProductID) // Quan trọng: Phải có OrderBy trước Skip/Take
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // THÊM PHƯƠNG THỨC ĐẾM TỔNG:
        public async Task<int> CountProductsAsync()
        {
            return await _context.Products.CountAsync(p => p.IsDelete == 0);
        }

    }
}
