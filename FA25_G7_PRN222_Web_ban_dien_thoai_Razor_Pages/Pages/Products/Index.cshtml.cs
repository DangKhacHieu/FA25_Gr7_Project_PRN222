using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;

        public IndexModel(IProductService productService)
        {
            _productService = productService;
        }

        // === BỘ LỌC ===
        // 1. Thuộc tính để "bắt" giá trị từ URL
        [BindProperty(SupportsGet = true)]
        public List<string> Brands { get; set; } = new List<string>();

        [BindProperty(SupportsGet = true)]
        public string? PriceRange { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<string> Rams { get; set; } = new List<string>();

        [BindProperty(SupportsGet = true)]
        public List<string> Roms { get; set; } = new List<string>();

        // === DANH SÁCH ĐỂ HIỂN THỊ ===
        public List<Product> ProductList { get; set; } = new List<Product>();

        // === DANH SÁCH ĐỂ TẠO BỘ LỌC (HTML) ===
        public List<string> AvailableBrands { get; set; } = new List<string>();
        public List<string> AvailableRams { get; set; } = new List<string>();
        public List<string> AvailableRoms { get; set; } = new List<string>();
        public Dictionary<string, string> PriceRanges { get; } = new Dictionary<string, string>
        {
            { "duoi-10t", "Dưới 10 triệu" },
            { "10-20t", "Từ 10 - 20 triệu" },
            { "tren-20t", "Trên 20 triệu" }
        };

        public async Task OnGetAsync()
        {
            // 1. Tải các tùy chọn lọc
            AvailableBrands = await _productService.GetDistinctBrandsAsync();
            AvailableRams = await _productService.GetDistinctRamsAsync();
            AvailableRoms = await _productService.GetDistinctRomsAsync();

            // 2. Bắt đầu xây dựng truy vấn
            var query = _productService.GetQueryableProducts();

            // 3. Lọc theo Hãng (nếu có)
            if (Brands.Count > 0)
            {
                query = query.Where(p => p.Brand != null && Brands.Contains(p.Brand));
            }

            // 4. Lọc theo RAM (nếu có)
            if (Rams.Count > 0)
            {
                query = query.Where(p => p.Ram != null && Rams.Contains(p.Ram));
            }

            // 5. Lọc theo ROM (nếu có)
            if (Roms.Count > 0)
            {
                query = query.Where(p => p.Rom != null && Roms.Contains(p.Rom));
            }

            // 6. Lọc theo Khoảng giá (nếu có)
            if (!string.IsNullOrEmpty(PriceRange))
            {
                switch (PriceRange)
                {
                    case "duoi-10t":
                        query = query.Where(p => p.Price < 10000000);
                        break;
                    case "10-20t":
                        query = query.Where(p => p.Price >= 10000000 && p.Price <= 20000000);
                        break;
                    case "tren-20t":
                        query = query.Where(p => p.Price > 20000000);
                        break;
                }
            }

            // 7. Thực thi truy vấn và lấy kết quả
            ProductList = await query.ToListAsync();
        }
    }
}