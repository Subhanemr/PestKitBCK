using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKit.DAL;
using PesKit.Models;
using PesKit.Utilities.Exceptions;

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
            if (id <= 0)  throw new WrongRequestException("The request sent does not exist"); 
            Project project = await _context.Projects.Include(p => p.ProjectImages).FirstOrDefaultAsync(pi => pi.Id == id);
            if (project == null)  throw new NotFoundException("Your request was not found"); 
            return View(project);
        } 
        
    }
}
