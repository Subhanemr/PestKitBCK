using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PesKit.DAL;
using PesKit.Models;
using PesKit.ViewModels;
using System.Security.Claims;

namespace PesKit.ViewComponents
{
    public class NavBarViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly UserManager<AppUser> _userManager;

        public NavBarViewComponent(AppDbContext context, IHttpContextAccessor http, UserManager<AppUser> userManager)
        {
            _context = context;
            _http = http;
            _userManager = userManager;
        }


        public async  Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
