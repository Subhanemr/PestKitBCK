using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKit.DAL;
using PesKit.Models;
using PesKit.ViewModels;

namespace PesKit.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Blog> blog = await _context.Blogs.Include(b => b.Author).ToListAsync();
            List<Employee> employees = await _context.Employees.Include(e => e.Position).Take(4).ToListAsync();

            HomeVM homeVM = new HomeVM { Blogs = blog, Employees = employees };
            return View(homeVM);
        }

        public async Task<IActionResult> About()
        {
            List<Employee> employees = await _context.Employees.Include(e => e.Position).Take(4).ToListAsync();
            return View(employees);
        }
    }
}
