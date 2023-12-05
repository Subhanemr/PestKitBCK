using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PesKit.DAL;

namespace PesKit.Areas.PesKitAdmin.Controllers
{
    [Area("PestKitAdmin")]
    [Authorize(Roles ="Admin,Moderator")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles ="Admin,Moderator")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
