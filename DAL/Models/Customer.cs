using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DAL.Models
{
    public class Customer 
    {
        public int CustomerId { get; set; }
        public string? UserName { get; set; }     // Cho phép null
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Password { get; set; }
        public int Status { get; set; }
        public string? Sex { get; set; }
        public DateTime? DOB { get; set; }        // Cũng cho phép null vì có thể khách chưa nhập
        public string? ImgCustomer { get; set; }

    }

}
