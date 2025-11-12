using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IFeedbackService _feedbackService;

        public DetailsModel(IProductService productService, IFeedbackService feedbackService)
        {
            _productService = productService;
            _feedbackService = feedbackService;
        }

        public Product? Product { get; set; }
        public List<Product> Variants { get; set; } = new();
        public List<Product> AvailableColors { get; set; } = new();
        public IEnumerable<Feedback> Feedbacks { get; set; } = new List<Feedback>();

        // Hàm OnGetAsync của bạn đã CHÍNH XÁC, giữ nguyên
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await _productService.GetProductByIdAsync(id);
            if (Product == null) return NotFound();

            var all = (await _productService.GetAllProductsAsync())
                .Where(p => p.ProductName == Product.ProductName)
                .ToList();

            // Dòng sản phẩm gồm tất cả màu và cấu hình
            Variants = all; // Chính xác
            AvailableColors = all; // Chính xác

            var feedbacks = await _feedbackService.GetFeedbacksForProductAsync(id);
            Feedbacks = feedbacks ?? Enumerable.Empty<Feedback>();

            return Page();
        }

        // === THAY THẾ HÀM NÀY ===
        public int GetProductIdByColorRamRom(string? color, string? ram, string? rom)
        {
            // 1. Ưu tiên tìm kiếm tổ hợp lý tưởng (ví dụ: Black, 8GB/128GB)
            // (Tôi đã bỏ .Concat(new[] { Product }) vì 'Variants' đã chứa tất cả)
            var idealMatch = Variants
                .FirstOrDefault(p => p.Color == color && p.Ram == ram && p.Rom == rom);

            if (idealMatch != null)
            {
                return idealMatch.ProductID; // Tìm thấy, trả về ngay
            }

            // 2. Logic dự phòng (Fallback) nếu không tìm thấy tổ hợp lý tưởng

            // Kịch bản A: Người dùng bấm vào RAM/ROM mới (ví dụ: 16GB/512GB)
            if (ram != Product.Ram || rom != Product.Rom)
            {
                // Tìm bất kỳ sản phẩm nào có RAM/ROM đó, bất kể màu sắc
                var ramRomFallback = Variants
                    .FirstOrDefault(p => p.Ram == ram && p.Rom == rom);
                if (ramRomFallback != null)
                {
                    return ramRomFallback.ProductID;
                }
            }
            // Kịch bản B: Người dùng bấm vào Màu mới (ví dụ: Blue)
            else if (color != Product.Color)
            {
                // Tìm bất kỳ sản phẩm nào có Màu đó, bất kể RAM/ROM
                var colorFallback = Variants
                    .FirstOrDefault(p => p.Color == color);
                if (colorFallback != null)
                {
                    return colorFallback.ProductID;
                }
            }

            // 3. Nếu mọi thứ thất bại, trả về ID hiện tại
            return Product.ProductID;
        }
    }
}