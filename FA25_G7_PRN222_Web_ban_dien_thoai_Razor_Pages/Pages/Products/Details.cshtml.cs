using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic; // Thêm
using System.Linq; // Thêm

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

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await _productService.GetProductByIdAsync(id);
            if (Product == null) return NotFound();
            var all = await _productService.GetAllProductsAsync();
            Variants = all.Where(p => p.ProductName == Product.ProductName && p.ProductID != id).ToList();
            AvailableColors = all.Where(p => p.ProductName == Product.ProductName && p.Color != Product.Color).ToList();

            var feedbacks = await _feedbackService.GetFeedbacksForProductAsync(id);
            Feedbacks = feedbacks ?? Enumerable.Empty<Feedback>();
            return Page();
        }
    }
}