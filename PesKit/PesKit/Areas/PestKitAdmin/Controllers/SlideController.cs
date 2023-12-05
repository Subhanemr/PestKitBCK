using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKit.Areas.PestKitAdmin.ViewModels;
using PesKit.DAL;
using PesKit.Models;

namespace PesKit.Areas.PestKitAdmin.Controllers
{
    [Area("PestKitAdmin")]
    [Authorize(Roles = "Admin,Moderator")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index()
        {
            List<Slide> slides = await _context.Slides.Include(s => s.Photo).ToListAsync();
            return View(slides);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSlideVM slideVM)
        {
            if (!ModelState.IsValid) { return View(slideVM); }
            return RedirectToAction(nameof(Index));


        }
    }
}
