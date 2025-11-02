using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Reply_Feedback
    {
        public int Reply_FeedbackID { get; set; }
        public int? FeedbackID { get; set; }
        public int? CustomerID { get; set; }
        public int? StaffID { get; set; }
        public string? Content_Reply { get; set; }

        // Navigation properties (nếu bạn dùng Entity Framework)
        public Feedback? Feedback { get; set; }
        public Customer? Customer { get; set; }
        public Staff? Staff { get; set; }
    }
}
