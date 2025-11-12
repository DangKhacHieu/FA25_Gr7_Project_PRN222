using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace DAL.Data
{
    public class PhoneContext : DbContext
    {
        public PhoneContext(DbContextOptions<PhoneContext> options)
            : base(options)
        {
        }

        public DbSet<Cart> Carts { get; set; } = default!;
        public DbSet<CartItem> CartItems { get; set; } = default!;
        public DbSet<Customer> Customers { get; set; } = default!;
        public DbSet<Feedback> Feedbacks { get; set; } = default!;
        //public DbSet<Import_Inventory> Import_Inventories { get; set; } = default!;
        public DbSet<Order_Details> Order_Details { get; set; } = default!;
        public DbSet<Order_List> Order_Lists { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<Reply_Feedback> Reply_Feedbacks { get; set; } = default!;
        public DbSet<Staff> Staffs { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order_List>()
                .HasKey(o => o.OrderID);

            // Cấu hình Khóa kép cho Order_Details
            modelBuilder.Entity<Order_Details>()
                 .HasKey(od => new { od.OrderID, od.ProductID });

            // 1. Cấu hình mối quan hệ giữa Order_Details và Order_List (Order_Details -> Order_List)
            modelBuilder.Entity<Order_Details>()
                .HasOne(od => od.Order_List)           // Chi tiết đơn hàng CÓ MỘT Order_List
                .WithMany(ol => ol.Order_Details!)     // Order_List CÓ NHIỀU Order_Details
                .HasForeignKey(od => od.OrderID)       // VÀ Cột khóa ngoại trên Order_Details là OrderID
                .OnDelete(DeleteBehavior.NoAction);    // Ngăn chặn xóa cascade (tùy chọn)

            // Đặt tên bảng trùng với database
            modelBuilder.Entity<Cart>().ToTable("Cart");
            modelBuilder.Entity<CartItem>().ToTable("CartItem");
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<Feedback>().ToTable("Feedback");
            //modelBuilder.Entity<Import_Inventory>().ToTable("Import_Inventory");
            modelBuilder.Entity<Order_Details>().ToTable("Order_Details");
            modelBuilder.Entity<Order_List>().ToTable("Order_List");
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<Reply_Feedback>().ToTable("Reply_Feedback");
            modelBuilder.Entity<Staff>().ToTable("Staff");
        }
    }
}
