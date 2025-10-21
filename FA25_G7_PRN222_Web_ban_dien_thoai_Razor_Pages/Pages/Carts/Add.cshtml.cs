using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages.Carts
{
    public class AddModel : PageModel
    {
        private readonly ICartService _cartService;
        public AddModel(ICartService cartService) => _cartService = cartService;

        [BindProperty] public int ProductId { get; set; }
        [BindProperty] public int Quantity { get; set; } = 1;

        public async Task<IActionResult> OnPostAsync()
        {
            int customerId = 1;
            await _cartService.AddToCartAsync(customerId, ProductId, Quantity);
            TempData["Message"] = "✅ Đã thêm sản phẩm vào giỏ hàng!";
            return RedirectToPage("Index");
        }
    }
}
