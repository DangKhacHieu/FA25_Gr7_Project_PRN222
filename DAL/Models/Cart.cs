using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Cart
    {
        public int CartID { get; set; }
        public int? CustomerID { get; set; }
        public int? ProductID { get; set; }
        public decimal? TotalPrice { get; set; }

        // Navigation properties (nếu bạn dùng Entity Framework)
        public Customer? Customer { get; set; }
        public Product? Product { get; set; }
    }
}
