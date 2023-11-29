using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKit.DAL;
using PesKit.Models;

namespace PesKit.Controllers
{
    public class ProjectController : Controller
    {
        private readonly AppDbContext _context;

        public ProjectController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Project> projects = await _context.Projects.Include(pi => pi.ProjectImages).ToListAsync();   
            return View(projects);
        }

        public async Task<IActionResult> More(int id)
        {
            if (id <= 0) { return BadRequest(); }
            Project project = await _context.Projects.Include(p => p.ProjectImages).FirstOrDefaultAsync(pi => pi.Id == id);
            if (project == null) { return NotFound(); }
            return View(project);
        } 
        
    }
}
