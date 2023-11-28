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
            CreateEmployeeVM employeeVM = new CreateEmployeeVM { 
            Departments = await _context.Departments.ToListAsync(),
            Positions = await _context.Positions.ToListAsync()
            };

            return View(employeeVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeVM employeeVM)
        {
            if (!ModelState.IsValid)
            {
                employeeVM.Departments = await _context.Departments.ToListAsync();
                employeeVM.Positions = await _context.Positions.ToListAsync();
                return View(employeeVM);
            }
            if (employeeVM.Photo is null)
            {
                employeeVM.Departments = await _context.Departments.ToListAsync();
                employeeVM.Positions = await _context.Positions.ToListAsync();
                ModelState.AddModelError("Photo", "The image must be uploaded");
                return View(employeeVM);
            }
            if (!employeeVM.Photo.ValiDataType())
            {
                employeeVM.Departments = await _context.Departments.ToListAsync();
                employeeVM.Positions = await _context.Positions.ToListAsync();
                ModelState.AddModelError("Photo", "File Not supported");
                return View(employeeVM);
            }
            if (!employeeVM.Photo.ValiDataSize(12))
            {
                employeeVM.Departments = await _context.Departments.ToListAsync();
                employeeVM.Positions = await _context.Positions.ToListAsync();
                ModelState.AddModelError("Photo", "Image should not be larger than 10 mb");
                return View(employeeVM);
            }

            string fileName = await employeeVM.Photo.CreateFile(_env.WebRootPath, "img");

            Employee employee = new Employee
            {
                ImgUrl = fileName,
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

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) { return BadRequest(); }
            Employee employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null) { return NotFound(); }
            UpdateEmployeeVM employeeVM = new UpdateEmployeeVM
            {
                DepartmentId = employee.DepartmentId,
                PositionId = employee.PositionId,
                Name = employee.Name,
                Surname = employee.Surname,
                InstLink = employee.InstLink,
                FaceLink = employee.FaceLink,
                TwitLink = employee.TwitLink,
                LinkedLink = employee.LinkedLink,
                ImgUrl = employee.ImgUrl,
                Departments = await _context.Departments.ToListAsync(),
                Positions = await _context.Positions.ToListAsync(),
        };

            return View(employeeVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateEmployeeVM employeeVM)
        {
            if (ModelState.IsValid)
            {
                employeeVM.Departments = await _context.Departments.ToListAsync();
                employeeVM.Positions = await _context.Positions.ToListAsync();
                return View(employeeVM);
            }
            Employee existed = _context.Employees.FirstOrDefault(b => b.Id == id);
            if (existed == null) { return NotFound(); }
            if (employeeVM.Photo is not null)
            {
                if (!employeeVM.Photo.ValiDataType())
                {
                    employeeVM.Departments = await _context.Departments.ToListAsync();
                    employeeVM.Positions = await _context.Positions.ToListAsync();
                    ModelState.AddModelError("Photo", "File Not supported");
                    return View(employeeVM);
                }
                if (!employeeVM.Photo.ValiDataSize(12))
                {
                    employeeVM.Departments = await _context.Departments.ToListAsync();
                    employeeVM.Positions = await _context.Positions.ToListAsync();
                    ModelState.AddModelError("Photo", "Image should not be larger than 10 mb");
                    return View(employeeVM);
                }
                string newImg = await employeeVM.Photo.CreateFile(_env.WebRootPath, "img");
                existed.ImgUrl.DeleteFile(_env.WebRootPath, "img");
                existed.ImgUrl = newImg;
            }
            existed.Name = employeeVM.Name;
            existed.Surname = employeeVM.Surname;
            existed.DepartmentId = employeeVM.DepartmentId;
            existed.PositionId = employeeVM.PositionId;
            existed.TwitLink = employeeVM.TwitLink;
            existed.InstLink = employeeVM.InstLink;
            existed.FaceLink = employeeVM.FaceLink;
            existed.LinkedLink = employeeVM.LinkedLink;


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) { return BadRequest(); }

            Employee employee = await _context.Employees.SingleOrDefaultAsync(e => e.Id == id);
            if (employee == null) { return NotFound(); }
            employee.ImgUrl.DeleteFile(_env.WebRootPath, "img");


            _context.Employees.Remove(employee);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> More(int id)
        {
            if (id <= 0) { return BadRequest(); };
            Employee employee = await _context.Employees.Include(e => e.Department).Include(e => e.Position).FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null) { return NotFound(); };

            return View(employee);
        }
    }
}
