using BLL;
using BLL.Interfaces;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FA25_G7_PRN222_Web_ban_dien_thoai.Controllers
{
    public class ProductsController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: Products
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var pagedResultProduct = await _productService.GetProductsPaginatedAsync(page, pageSize);

            // ÉP KIỂU TẠI ĐÂY VÀ TRẢ VỀ:
            var pagedResultObject = new DAL.Models.PagedResult<object> // Giả sử PagedResult nằm ở DAL.Models
            {
                Items = pagedResultProduct.Items.Cast<object>(), // Ép kiểu danh sách items
                TotalCount = pagedResultProduct.TotalCount,
                PageIndex = pagedResultProduct.PageIndex,
                PageSize = pagedResultProduct.PageSize
            };

            return View(pagedResultObject); // Trả về kiểu PagedResult<object>
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,ProductName,Price,ImageURL,Brand,Ram,Rom,Color,Operating_System_name,Size,Chip_name,GPU_name,Camera_Front,Camera_Behind,Operating_system_version,Refresh_rate,Screen_resolution")] Product product)
        {
            var existingProducts = await _productService.GetAllProductsAsync();
            var isDuplicate = existingProducts.Any(p =>
                p.ProductName == product.ProductName &&
                p.Color == product.Color &&
                p.Rom == product.Rom &&
                (p.IsDelete == null || p.IsDelete == 0)); // Chỉ kiểm tra sản phẩm chưa xóa

            if (isDuplicate)
            {
                ModelState.AddModelError("ProductName", "Sản phẩm với cùng tên, màu và Rom đã tồn tại. Vui lòng chọn thông tin khác.");
                return View(product);
            }
            if (ModelState.IsValid)
            {
                await _productService.CreateProductAsync(product);
                TempData["SuccessMessage"] = $"Đã thêm sản phẩm '{product.ProductName}' thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,ProductName,Price,ImageURL,Brand,Ram,Rom,Color,Operating_System_name,Size,Chip_name,GPU_name,Quantity_Sell,Quantity_Product,Camera_Front,Camera_Behind,Operating_system_version,Refresh_rate,Screen_resolution,IsDelete")] Product product)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.UpdateProductAsync(product);
                    TempData["SuccessMessage"] = $"Đã cập nhật sản phẩm '{product.ProductName}' thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _productService.ProductExistsAsync(product.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product != null)
            {
                product.IsDelete = 1; // Cập nhật trạng thái IsDelete thành 1
                await _productService.UpdateProductAsync(product);
                TempData["SuccessMessage"] = "Xóa thành công"; // Gửi thông báo thành công
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
