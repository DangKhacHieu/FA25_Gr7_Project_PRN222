using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public int? Price { get; set; }
        public string? ImageURL { get; set; }
        public string? Brand { get; set; }
        public string? Ram { get; set; }
        public string? Rom { get; set; }
        public string? Color { get; set; }
        public string? Operating_System_name { get; set; }
        public string? Size { get; set; }
        public string? Chip_name { get; set; }
        public string? GPU_name { get; set; }
        public int? Quantity_Sell { get; set; }
        public int? Quantity_Product { get; set; }
        public string? Camera_Front { get; set; }
        public string? Camera_Behind { get; set; }
        public string? Operating_system_version { get; set; }
        public string? Refresh_rate { get; set; }
        public string? Screen_resolution { get; set; }
        public int IsDelete { get; set; }
    }
}
