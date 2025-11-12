using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Interfaces;
using DAL.Models;

namespace DAL.Repositories
{
    public class ImportProductRepository:IImportProductRepository
    {
        private readonly PhoneContext _context;

        public ImportProductRepository(PhoneContext context)
        {
            _context = context;
        }

        public void AddOrUpdateProducts(List<Product> products)
        {
            foreach (var p in products)
            {
                // Kiểm tra sản phẩm tồn tại dựa trên ProductName + Brand
                var exist = _context.Products
                    .FirstOrDefault(x => x.ProductName == p.ProductName && x.Brand == p.Brand && x.IsDelete == 0);

                if (exist != null)
                {
                    // Cập nhật thông tin và tăng Quantity_Product
                    exist.Price = p.Price;
                    exist.Ram = p.Ram;
                    exist.Rom = p.Rom;
                    exist.Color = p.Color;
                    exist.Operating_System_name = p.Operating_System_name;
                    exist.Size = p.Size;
                    exist.Chip_name = p.Chip_name;
                    exist.GPU_name = p.GPU_name;
                    exist.Camera_Front = p.Camera_Front;
                    exist.Camera_Behind = p.Camera_Behind;
                    exist.Operating_system_version = p.Operating_system_version;
                    exist.Refresh_rate = p.Refresh_rate;
                    exist.Screen_resolution = p.Screen_resolution;

                    exist.Quantity_Product = (exist.Quantity_Product ?? 0) + (p.Quantity_Product ?? 0);
                }
                else
                {
                    // Thêm mới sản phẩm
                    if (p.Quantity_Product == null) p.Quantity_Product = 0;
                    if (p.Quantity_Sell == null) p.Quantity_Sell = 0;
                    _context.Products.Add(p);
                }
            }

            _context.SaveChanges();
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products.Where(x => x.IsDelete == 0).ToList();
        }
    }
}

