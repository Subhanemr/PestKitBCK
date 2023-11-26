using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKit.Areas.PestKitAdmin.ViewModels;
using PesKit.DAL;
using PesKit.Models;
using PesKit.Utilities.Validata;

namespace PesKit.Areas.PestKitAdmin.Controllers
{
    [Area("PestKitAdmin")]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Employee> employees = await _context.Employees.Include(e => e.Department).Include(e => e.Position).ToListAsync();
            return View(employees);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.Positions = await _context.Positions.ToListAsync();

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeVM employeeVM)
        {
            if (employeeVM.Photo is null)
            {
                ViewBag.Departments = await _context.Departments.ToListAsync();
                ViewBag.Positions = await _context.Positions.ToListAsync();
                ModelState.AddModelError("Photo", "The image must be uploaded");
                return View(employeeVM);
            }
            if (!employeeVM.Photo.ValiDataType())
            {
                ViewBag.Departments = await _context.Departments.ToListAsync();
                ViewBag.Positions = await _context.Positions.ToListAsync();
                ModelState.AddModelError("Photo", "File Not supported");
                return View(employeeVM);
            }
            if (!employeeVM.Photo.ValiDataSize(12))
            {
                ViewBag.Departments = await _context.Departments.ToListAsync();
                ViewBag.Positions = await _context.Positions.ToListAsync();
                ModelState.AddModelError("Photo", "Image should not be larger than 10 mb");
                return View(employeeVM);
            }

            string fileName = await employeeVM.Photo.CreateFile(_env.WebRootPath, "img");

            Employee employee = new Employee 
            { 
                ImgUrl= fileName,
                Name = employeeVM.Name,
                Surname = employeeVM.Surname,
                DepartmentId = employeeVM.DepartmentId,
                PositionId = employeeVM.PositionId,
                InstLink = employeeVM.InstLink,
                FaceLink = employeeVM.FaceLink,
                TwitLink = employeeVM.TwitLink,
                LinkedLink = employeeVM.LinkedLink
            };

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
