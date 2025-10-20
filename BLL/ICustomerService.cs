using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace BLL
{
    public interface ICustomerService
    {
        List<Customer> GetAllCustomers();
        Customer GetCustomerById(int id);
    }
}
