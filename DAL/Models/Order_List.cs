using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Order_List
    {
        public int OrderID { get; set; }
        public int? CustomerID { get; set; }
        public int? StaffID { get; set; }
        public string? Address { get; set; }
        public DateTime? Date { get; set; }
        public string? Status { get; set; }
        public string? PhoneNumber { get; set; }
        public int? Total { get; set; }

        // Khai báo mối quan hệ 1-n: Một Order_List có nhiều Order_Details
        public ICollection<Order_Details>? Order_Details { get; set; }
        // Navigation properties (tuỳ chọn, nếu muốn ánh xạ quan hệ)
        public Customer? Customer { get; set; }
        public Staff? Staff { get; set; }
        // Khởi tạo List trong constructor để tránh NullReferenceException
        public Order_List()
        {
            Order_Details = new List<Order_Details>();
        }
    }
}
