
using BLL.Interfaces;
using BLL.IServices;
using BLL.Services;
using DAL.Data;
using DAL.Interfaces;
using DAL.IRepositories;
using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// 1. THÊM BỘ NHỚ CACHE CHO SESSION
builder.Services.AddDistributedMemoryCache();

// 2. THÊM DỊCH VỤ SESSION
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian chờ
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();
// 🔌 Kết nối database
builder.Services.AddDbContext<PhoneContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PhoneStoreContext")));

// 🧠 Inject tầng BLL và tầng DAL
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();
app.UseSession();
app.MapRazorPages()
   .WithStaticAssets();
app.MapGet("/", context =>
{
    context.Response.Redirect("/Home");
    return Task.CompletedTask;
});
app.Run();
