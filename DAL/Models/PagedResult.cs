using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } // Dữ liệu của trang hiện tại
        public int TotalCount { get; set; }       // Tổng số bản ghi (toàn bộ)
        public int PageIndex { get; set; }        // Trang hiện tại
        public int PageSize { get; set; }         // Số lượng bản ghi trên mỗi trang

        // Thuộc tính tính toán
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
