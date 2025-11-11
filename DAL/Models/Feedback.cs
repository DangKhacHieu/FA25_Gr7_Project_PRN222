using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Feedback
    {
        public int FeedbackID { get; set; }
        public int? CustomerID { get; set; }
        public string? Content { get; set; }
        public int? RatePoint { get; set; }
        public int? ProductID { get; set; }

        // Navigation properties (nếu dùng Entity Framework)
        public Customer? Customer { get; set; }
        public Product? Product { get; set; }
    }
}