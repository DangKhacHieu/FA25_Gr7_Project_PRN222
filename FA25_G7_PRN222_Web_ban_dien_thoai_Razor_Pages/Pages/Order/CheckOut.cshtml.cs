using BLL.Interfaces;
using DAL.Interfaces;
using DAL.IRepositories;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations; // Cần cho [Required]
using System.Linq;
using System.Threading.Tasks;

// Giả định namespace của bạn
namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly ICartRepository _cartRepo;
        private readonly ICustomerRepository _customerRepo; // Để điền trước thông tin

        public CheckoutModel(
            IOrderService orderService,
            ICartRepository cartRepo,
            ICustomerRepository customerRepo) // Bạn cần inject ICustomerRepository
        {
            _orderService = orderService;
            _cartRepo = cartRepo;
            _customerRepo = customerRepo;
        }

        // --- Thuộc tính để hiển thị ra View ---
        public Cart? Cart { get; set; } // Giữ toàn bộ giỏ hàng (bao gồm Items và TotalPrice)

        // --- Thuộc tính nhận dữ liệu từ Form ---

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        public string Address { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        [BindProperty]
        public string PaymentMethod { get; set; } = "COD"; // Mặc định là COD

        // Hàm helper lấy CustomerID từ Session
        private int? GetCurrentCustomerId()
        {
            return HttpContext.Session.GetInt32("CustomerId");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            int? customerId = GetCurrentCustomerId();
            if (customerId == null)
            {
                // Chưa đăng nhập, về trang login
                return RedirectToPage("/Login");
            }

            // 1. Lấy giỏ hàng (Giả định GetCartByCustomerAsync đã Include CartItems và Product)
            Cart = await _cartRepo.GetCartByCustomerAsync(customerId.Value);

            // 2. Kiểm tra giỏ hàng
            if (Cart == null || Cart.CartItems == null || !Cart.CartItems.Any())
            {
                // Giỏ hàng trống, quay về trang giỏ hàng (hoặc trang chủ)
                TempData["Message"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToPage("/Cart");
            }

            // 3. Lấy thông tin khách hàng để điền sẵn
            var customer = await _customerRepo.GetCustomerByIdAsync(customerId.Value);
            if (customer != null)
            {
                Address = customer.Address ?? string.Empty;
                PhoneNumber = customer.PhoneNumber ?? string.Empty;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            int? customerId = GetCurrentCustomerId();
            if (customerId == null)
            {
                return RedirectToPage("/Login");
            }

            // 1. Kiểm tra validation (Address, Phone có bị trống không)
            if (!ModelState.IsValid)
            {
                // Nếu form không hợp lệ, tải lại giỏ hàng và hiển thị lại trang
                Cart = await _cartRepo.GetCartByCustomerAsync(customerId.Value);
                return Page();
            }

            try
            {
                // 2. Gọi Service để xử lý logic (Transaction, trừ kho, xóa giỏ hàng...)
                await _orderService.CreateOrderFromCartAsync(
                    customerId.Value,
                    Address,
                    PhoneNumber,
                    PaymentMethod
                );

                // 3. Đặt hàng thành công
                TempData["SuccessMessage"] = "Đặt hàng thành công!";

                // Trả về trang Lịch sử đơn hàng như bạn yêu cầu
                return RedirectToPage("/Order/History");
            }
            catch (Exception ex)
            {
                // 4. Xử lý lỗi (ví dụ: Hết hàng, Lỗi DB...)
                // Gửi lỗi này ra View để người dùng thấy
                ModelState.AddModelError(string.Empty, ex.Message);

                // Nếu lỗi, phải tải lại giỏ hàng để hiển thị lại trang
                Cart = await _cartRepo.GetCartByCustomerAsync(customerId.Value);
                return Page();
            }
        }
    }
}