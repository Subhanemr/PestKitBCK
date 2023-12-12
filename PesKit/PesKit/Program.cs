using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PesKit.DAL;
using PesKit.Interfaces;
using PesKit.LayoutService;
using PesKit.Middlewares;
using PesKit.Models;
using PesKit.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>((options) => { options.UseSqlServer(builder.Configuration.GetConnectionString("Default")); });

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;

    options.User.RequireUniqueEmail = true;
    
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

    options.SignIn.RequireConfirmedEmail = true;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddScoped<ServicesLayout>();
builder.Services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();
builder.Services.AddScoped<IEmailService, EmailService>();


var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.MapControllerRoute(
    "Default",
    "{area}/{controller=home}/{action=index}/{id?}");

app.MapControllerRoute(
    "Default",
    "{controller=home}/{action=index}/{id?}");


app.Run();
