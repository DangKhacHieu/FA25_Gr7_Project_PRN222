using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;
using BLL.Common;
using System.Collections.Generic;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages.Order
{
    public class DetailsModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IFeedbackService _feedbackService;

        public DetailsModel(IOrderService orderService, IFeedbackService feedbackService)
        {
            _orderService = orderService;
            _feedbackService = feedbackService;
        }

        public Order_List OrderDetails { get; set; } = new();
        public List<int> AlreadyFeedbackProducts { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToPage("/Login");

            OrderDetails = await _orderService.GetOrderDetailsForCustomerAsync(Id, customerId.Value);
            if (OrderDetails == null) return NotFound();

            if (OrderDetails.Status != null && OrderDetails.Status.Contains("Completed"))
            {
                AlreadyFeedbackProducts = (await _feedbackService.GetAllFeedbacksAsync())
                    .Where(f => f.CustomerID == customerId)
                    .Select(f => f.ProductID ?? 0)
                    .Distinct() 
                    .ToList();
            }

            return Page();
        }
        
        public async Task<IActionResult> OnPostAddFeedbackAsync(int ProductId, int OrderId, string Content, int RatePoint)
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToPage("/Login");

            var eligibility = await _feedbackService.CheckFeedbackEligibilityAsync(customerId.Value, OrderId, ProductId);

            if (eligibility != IFeedbackService.FeedbackEligibility.Eligible)
            {
                if (eligibility == IFeedbackService.FeedbackEligibility.AlreadySubmitted)
                    TempData["Error"] = "Bạn đã đánh giá sản phẩm này rồi.";
                else
                    TempData["Error"] = "Đơn hàng chưa hoàn thành, bạn không thể đánh giá.";

                return RedirectToPage(new { id = OrderId });
            }

            var result = await _feedbackService.AddFeedbackAsync(customerId.Value, ProductId, Content, RatePoint);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToPage(new { id = OrderId });
        }
    }
}