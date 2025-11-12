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
        public async Task IncreaseProductQuantityAsync(int productId, int quantityToAdd)
        {
            // Giả định IProductRepository có phương thức tương ứng
            await _productRepo.IncreaseProductQuantityAsync(productId, quantityToAdd);
        }
        public async Task<DAL.Models.PagedResult<Product>> GetProductsPaginatedAsync(int pageIndex, int pageSize)
        {
            if (pageIndex < 1) pageIndex = 1;

            // 1. Đếm tổng số bản ghi
            var totalCount = await _productRepo.CountProductsAsync();

            // 2. Lấy dữ liệu của trang hiện tại
            var items = await _productRepo.GetPagedProductsAsync(pageIndex, pageSize);

            // 3. Trả về PagedResult
            return new DAL.Models.PagedResult<Product>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task DecreaseProductQuantityAsync(int productId, int quantityToDecrease)
        {
            // Bạn cần một hàm GetById trong IProductRepository (có thể bạn đã có)
            var product = await _productRepo.GetByIdAsync(productId);

            if (product == null)
            {
                throw new KeyNotFoundException($"Sản phẩm với ID {productId} không tồn tại.");
            }

            // Giả sử tên cột số lượng là QuantityProduct (dựa theo DB)
            if (product.Quantity_Product < quantityToDecrease)
            {
                // Ném lỗi này sẽ ngăn chặn transaction thành công
                throw new InvalidOperationException($"Không đủ hàng cho sản phẩm '{product.ProductName}'.");
            }

            product.Quantity_Product -= quantityToDecrease;

            // Tái sử dụng hàm UpdateAsync
            await _productRepo.UpdateAsync(product);
        }
    }
}
