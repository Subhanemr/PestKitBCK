using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKit.DAL;
using PesKit.Models;

namespace PesKit.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly AppDbContext _context;

        public DepartmentController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Department> department = await _context.Departments.ToListAsync();
            return View(department);
        }
        public async Task<IActionResult> Detail(int id)
        {
            Department department = await _context.Departments.Include(d => d.Employees).ThenInclude(e => e.Position).FirstOrDefaultAsync(d => d.Id == id);
            return View(department);
        }
    }
}
