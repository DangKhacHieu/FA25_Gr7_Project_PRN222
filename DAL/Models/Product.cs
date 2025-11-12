using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Product
    {
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Tên sản phẩm không được quá 255 ký tự.")]
        [Display(Name = "Tên sản phẩm")]
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc.")]
        [Range(0, 1000000000, ErrorMessage = "Giá phải lớn hơn 0.")] // Giới hạn giá trị
        [Display(Name = "Giá")]
        public int? Price { get; set; }

        [Display(Name = "URL hình ảnh")]
        [DataType(DataType.ImageUrl)]
        public string? ImageURL { get; set; }

        [Required(ErrorMessage = "Thương hiệu là bắt buộc.")]
        [Display(Name = "Thương hiệu")]
        public string? Brand { get; set; }

        [Display(Name = "RAM")]
        public string? Ram { get; set; }

        [Display(Name = "Bộ nhớ trong (ROM)")]
        public string? Rom { get; set; }

        [Display(Name = "Màu sắc")]
        public string? Color { get; set; }

        [Display(Name = "Hệ điều hành")]
        public string? Operating_System_name { get; set; }

        [Display(Name = "Kích thước màn hình")]
        public string? Size { get; set; }

        [Display(Name = "Chip (CPU)")]
        public string? Chip_name { get; set; }

        [Display(Name = "GPU")]
        public string? GPU_name { get; set; }

        [Display(Name = "Số lượng đã bán")]
        public int? Quantity_Sell { get; set; }

        [Display(Name = "Số lượng còn")]
        public int? Quantity_Product { get; set; }

        [Display(Name = "Camera trước")]
        public string? Camera_Front { get; set; }

        [Display(Name = "Camera sau")]
        public string? Camera_Behind { get; set; }

        [Display(Name = "Phiên bản HĐH")]
        public string? Operating_system_version { get; set; }

        [Display(Name = "Tần số quét")]
        public string? Refresh_rate { get; set; }

        [Display(Name = "Độ phân giải")]
        public string? Screen_resolution { get; set; }

        // Không cần [Display] vì đây là trường nội bộ (Soft Delete)
        public int IsDelete { get; set; }
    }
}
