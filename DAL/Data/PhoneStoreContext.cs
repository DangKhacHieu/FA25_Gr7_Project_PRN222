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
        //public DbSet<Feedback> Feedbacks { get; set; } = default!;
        //public DbSet<Import_Inventory> Import_Inventories { get; set; } = default!;
        //public DbSet<Order_Details> Order_Details { get; set; } = default!;
        //public DbSet<Order_List> Order_Lists { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;
        //public DbSet<Reply_Feedback> Reply_Feedbacks { get; set; } = default!;
        //public DbSet<Staff> Staffs { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Đặt tên bảng trùng với database
            modelBuilder.Entity<Cart>().ToTable("Cart");
            modelBuilder.Entity<CartItem>().ToTable("CartItem");
            modelBuilder.Entity<Customer>().ToTable("Customer");
            //modelBuilder.Entity<Feedback>().ToTable("Feedback");
            //modelBuilder.Entity<Import_Inventory>().ToTable("Import_Inventory");
            //modelBuilder.Entity<Order_Details>().ToTable("Order_Details");
            //modelBuilder.Entity<Order_List>().ToTable("Order_List");
            modelBuilder.Entity<Product>().ToTable("Product");
            //modelBuilder.Entity<Reply_Feedback>().ToTable("Reply_Feedback");
            //modelBuilder.Entity<Staff>().ToTable("Staff");
        }
    }
}
