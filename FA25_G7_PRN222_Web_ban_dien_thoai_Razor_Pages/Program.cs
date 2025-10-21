
using DAL.Data;
using DAL.Repositories;
using DAL.Models;
using BLL.Services;
using Microsoft.EntityFrameworkCore;
using DAL.IRepositories;
using BLL.IServices;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// 🔌 Kết nối database
builder.Services.AddDbContext<PhoneContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PhoneStoreContext")));

// 🧠 Inject tầng BLL
builder.Services.AddScoped<ICustomerRepository, DAL.Repositories.CustomerRepository>(); // ✅ THÊM DÒNG NÀY
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<CustomerService>();

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

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
