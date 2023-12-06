using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PesKit.DAL;
using PesKit.Models;

namespace PesKit.ViewComponents
{
    public class TopBarViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly UserManager<AppUser> _userManager;


        public TopBarViewComponent(AppDbContext context, IHttpContextAccessor http, UserManager<AppUser> userManager)
        {
            _context = context;
            _http = http;
            _userManager = userManager;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            AppUser appUser = new AppUser();


            if (User.Identity.IsAuthenticated)
            {
                appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            }

            return View(appUser);
        }
    }
}
