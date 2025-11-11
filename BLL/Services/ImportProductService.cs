using System;
using System.Collections.Generic;
using System.IO;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using OfficeOpenXml;

namespace BLL.Services
{
    public class ImportProductService : IImportProductService
    {
        private readonly IImportProductRepository _repo;

        public ImportProductService(IImportProductRepository repo)
        {
            _repo = repo;
        }

        // Thêm/Sửa thủ công
        public void AddOrUpdateProductManual(Product product)
        {
            if (product.Quantity_Product == null) product.Quantity_Product = 0;
            if (product.Quantity_Sell == null) product.Quantity_Sell = 0;
            _repo.AddOrUpdateProducts(new List<Product> { product });
        }

        // Lấy danh sách
        public List<Product> GetAllProducts()
        {
            return _repo.GetAllProducts();
        }
    }
}
