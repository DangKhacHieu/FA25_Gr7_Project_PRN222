using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages.Products
{
    public class SearchModel : PageModel
    {
        private readonly IProductService _productService;

        public SearchModel(IProductService productService)
        {
            _productService = productService;
        }

        public IEnumerable<Product> Products { get; set; } = new List<Product>();

        [BindProperty(SupportsGet = true)]
        public string q { get; set; } = string.Empty;

        // 1. Handler cho trang kết quả đầy đủ (khi submit form)
        public async Task<IActionResult> OnGetAsync()
        {
            if (!string.IsNullOrWhiteSpace(q))
            {
                Products = await _productService.SearchProductsAsync(q);
            }
            return Page();
        }

        // 2. Handler cho Popup AJAX (chỉ trả về JSON)
        public async Task<IActionResult> OnGetSuggestionsAsync()
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 3)
            {
                return new JsonResult(new List<object>());
            }

            var products = await _productService.SearchProductsAsync(q);

            // Chỉ chọn 5 kết quả cho popup và dữ liệu cần thiết
            var suggestions = products
                .Select(p => new {
                    p.ProductID,
                    p.ProductName,
                    p.ImageURL,
                    Price = p.Price?.ToString("N0") // Format giá
                })
                .Take(5);

            return new JsonResult(suggestions);
        }
    }
}
