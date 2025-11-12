using BLL.Interfaces;
using BLL.IServices;
using BLL.Services;
using DAL.Data;
using DAL.Interfaces;
using DAL.IRepositories;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 🔌 Kết nối database
builder.Services.AddDbContext<PhoneContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PhoneStoreContext")));

builder.Services.AddScoped<DbContext>(provider => provider.GetService<PhoneContext>()!);
// 🧠 Inject tầng BLL
builder.Services.AddScoped<ICustomerRepository,CustomerRepository>(); // ✅ THÊM DÒNG NÀY
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<BLL.Interfaces.IStaffService, BLL.Services.StaffService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<StaffRepository>();
builder.Services.AddScoped<StaffService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian sống của Session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

// ✅ Test kết nối DB ở đây
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PhoneContext>();
    try
    {
        if (dbContext.Database.CanConnect())
        {
            Console.WriteLine("✅ Kết nối Database thành công!");
        }
        else
        {
            Console.WriteLine("❌ Không thể kết nối tới Database!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Lỗi khi kết nối Database: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Staffs}/{action=Login}/{id?}");

app.Run();
