using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task CreateProductAsync(Product product)
        {
            product.Quantity_Product = 0;
            product.Quantity_Sell = 0;
            product.IsDelete = 0;
            await _productRepo.AddAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepo.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepo.DeleteAsync(id);
        }

        public async Task<bool> ProductExistsAsync(int id)
        {
            return await _productRepo.ExistsAsync(id);
        }

        public IQueryable<Product> GetQueryableProducts()
            => _productRepo.GetQueryableProducts();

        public async Task<List<string>> GetDistinctBrandsAsync()
            => await _productRepo.GetDistinctBrandsAsync();

        public async Task<List<string>> GetDistinctRamsAsync()
            => await _productRepo.GetDistinctRamsAsync();

        public async Task<List<string>> GetDistinctRomsAsync()
            => await _productRepo.GetDistinctRomsAsync();
    }
}
