using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Xóa toàn bộ session
            HttpContext.Session.Clear();

            // Xóa cookie (nếu có dùng cookie để lưu login)
            foreach (var cookieKey in HttpContext.Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookieKey);
            }

            // Chuyển hướng về trang login
            return RedirectToPage("/Login");
        }
    }
}
