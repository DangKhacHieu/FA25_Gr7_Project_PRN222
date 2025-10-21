using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Staff
    {
        public int StaffID { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Username { get; set; }
        public string? CCCD { get; set; }
        public string? Province_City { get; set; }
        public DateTime? DOB { get; set; }
        public string? Sex { get; set; }
        public int Status { get; set; }
    }
}
