using BLL.Interfaces;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_MVC.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _feedbackService;
        private readonly PhoneContext _context; 

        public FeedbackController(IFeedbackService feedbackService, PhoneContext context)
        {
            _feedbackService = feedbackService;
            _context = context;
        }

        public IActionResult Index()
        {
            var feedbacks = _feedbackService.GetAllFeedbacks();

            // ✅ Lấy danh sách FeedbackID đã có reply
            var repliedFeedbackIds = _context.Reply_Feedbacks
                .Select(r => r.FeedbackID)
                .Distinct()
                .ToList();

            // ✅ Tạo dictionary: FeedbackID → true/false (đã rep hay chưa)
            var replyStatus = feedbacks.ToDictionary(
                f => f.FeedbackID,
                f => repliedFeedbackIds.Contains(f.FeedbackID)
            );

            ViewBag.ReplyStatus = replyStatus;

            return View(feedbacks);
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            _feedbackService.DeleteFeedback(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Reply(int id)
        {
            var feedback = _feedbackService.GetFeedbackById(id);
            if (feedback == null)
                return NotFound();

            // ✅ Lấy tên Customer an toàn
            var customerName = _context.Customers
                .Where(c => c.CustomerId == (feedback.CustomerID ?? 0))
                .Select(c => c.UserName)
                .FirstOrDefault() ?? "Unknown";
            ViewBag.CustomerName = customerName;

            // ✅ Lấy danh sách reply
            var replies = _feedbackService.GetRepliesByFeedbackId(id);

            // ✅ Tạo dictionary StaffID → Username (tránh null)
            var staffNames = _context.Staffs
                .Where(s => s.StaffID != null && s.Username != null)
                .Select(s => new
                {
                    Id = s.StaffID,
                    Name = s.Username ?? "Unknown"
                })
                .ToDictionary(s => s.Id, s => s.Name);

            ViewBag.StaffNames = staffNames;
            ViewBag.Replies = replies;

            return View(feedback);
        }

        [HttpPost]
        public IActionResult Reply(int id, string replyContent)
        {
            if (string.IsNullOrWhiteSpace(replyContent))
            {
                TempData["Error"] = "Reply content cannot be empty.";
                return RedirectToAction("Reply", new { id });
            }

            // ✅ Tạm thời set StaffID cứng để test (vd: staff có ID = 1)
            int staffId = 1;

            // int.TryParse(HttpContext.Session.GetString("StaffID"), out int staffId);
            //if (staffId == 0)
            //{
            //    TempData["Error"] = "You must log in as staff to reply feedback.";
            //    return RedirectToAction("Login", "Staff");
            //}


            var fb = _feedbackService.GetFeedbackById(id);
            if (fb == null)
                return NotFound();

            var reply = new Reply_Feedback
            {
                FeedbackID = fb.FeedbackID,
                CustomerID = fb.CustomerID,
                StaffID = staffId,
                Content_Reply = replyContent
            };

            _feedbackService.AddReplyFeedback(reply);
            TempData["Success"] = "Reply added successfully!";
            return RedirectToAction("Index", new { id });
        }

    }
}
