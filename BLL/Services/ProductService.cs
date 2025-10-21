using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;

namespace BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        public ProductService(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
            => await _productRepo.GetAllAsync();

        public async Task<Product?> GetProductByIdAsync(int id)
            => await _productRepo.GetByIdAsync(id);

        public async Task<IEnumerable<Product>> SearchProductsAsync(string keyword)
            => await _productRepo.SearchByNameAsync(keyword);

        public async Task AddProductAsync(Product product)
            => await _productRepo.AddAsync(product);

        public async Task UpdateProductAsync(Product product)
            => await _productRepo.UpdateAsync(product);

        public async Task DeleteProductAsync(int id)
            => await _productRepo.DeleteAsync(id);
    }
}
