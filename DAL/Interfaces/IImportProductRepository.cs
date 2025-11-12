using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Interfaces
{
    public interface IImportProductRepository
    {
        void AddOrUpdateProducts(List<Product> products);
        List<Product> GetAllProducts();
    }
}
