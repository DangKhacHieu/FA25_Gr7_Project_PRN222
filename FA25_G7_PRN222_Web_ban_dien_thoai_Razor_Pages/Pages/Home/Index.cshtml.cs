using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages.Home
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;

        public IndexModel(IProductService productService)
        {
            _productService = productService;
        }

        // Danh sách để hiển thị trên giao diện
        public List<Product> BestSellers { get; set; } = new List<Product>();
        public List<Product> NewArrivals { get; set; } = new List<Product>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Lấy tất cả sản phẩm
            var allProducts = (await _productService.GetAllProductsAsync()).ToList();

            // 1. Lấy 8 sản phẩm bán chạy nhất (Quantity_Sell cao nhất)
            BestSellers = allProducts
                .OrderByDescending(p => p.Quantity_Sell)
                .Take(8)
                .ToList();

            // 2. Lấy 8 sản phẩm mới nhất (ProductID lớn nhất)
            NewArrivals = allProducts
                .OrderByDescending(p => p.ProductID)
                .Take(8)
                .ToList();

            return Page();
        }
    }
}