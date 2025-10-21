using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Import_Inventory
    {
        public int Import_InventoryID { get; set; }
        public int? Product_ID { get; set; }
        public int? Import_Price { get; set; }
        public DateTime? Date { get; set; }
        public int? Import_Quantity { get; set; }
        public string? Supplier { get; set; }

        // Navigation property (nếu bạn dùng Entity Framework)
        public Product? Product { get; set; }
    }
}
