using Microsoft.AspNetCore.Mvc;
using PesKit.DAL;

namespace PesKit.Areas.PesKitAdmin.Controllers
{
    [Area("PestKitAdmin")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
