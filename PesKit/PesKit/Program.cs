using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PesKit.DAL;
using PesKit.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>((options) => { options.UseSqlServer(builder.Configuration.GetConnectionString("Default")); });
builder.Services.AddHttpContextAccessor();

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;

    options.User.RequireUniqueEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

var app = builder.Build();

app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();
app.UseStaticFiles();

app.MapControllerRoute(
    "Default",
    "{area}/{controller=home}/{action=index}/{id?}");

app.MapControllerRoute(
    "Default",
    "{controller=home}/{action=index}/{id?}");


app.Run();
