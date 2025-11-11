using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Order_Details
    {
        public int OrderDetailID { get; set; }
        public int? Quantity { get; set; }
        public int ProductID { get; set; }
        public int OrderID { get; set; }

        // Quan hệ khóa ngoại (nếu dùng Entity Framework)
        public Product? Product { get; set; }
        public Order_List? Order_List { get; set; }
    }
}
