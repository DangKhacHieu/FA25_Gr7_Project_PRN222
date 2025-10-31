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

// 🧠 Inject tầng BLL
builder.Services.AddScoped<ICustomerRepository,CustomerRepository>(); // ✅ THÊM DÒNG NÀY
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<StaffRepository>();
builder.Services.AddScoped<StaffService>();
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
