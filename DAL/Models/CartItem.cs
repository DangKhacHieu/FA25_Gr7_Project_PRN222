using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class CartItem
    {
        public int CartItemID { get; set; }
        public int? CustomerID { get; set; }
        public int? ProductID { get; set; }
        public decimal? SubTotal { get; set; }
        public int? Quantity { get; set; }
        public int? CartID { get; set; }

        // Navigation properties (nếu bạn dùng Entity Framework)
        public Customer? Customer { get; set; }
        public Product? Product { get; set; }
        public Cart? Cart { get; set; }
    }
}
