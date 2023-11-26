using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKit.DAL;
using PesKit.Models;

namespace PesKit.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;

        public BlogController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Blog> blogs = await _context.Blogs.Include(b => b.Author).ToListAsync();
            return View(blogs);
        }
    }
}
