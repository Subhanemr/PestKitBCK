using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKit.Areas.PestKitAdmin.ViewModels;
using PesKit.DAL;
using PesKit.Models;
using PesKit.Utilities.Validata;

namespace PesKit.Areas.PestKitAdmin.Controllers
{
    [Area("PestKitAdmin")]
    public class DepartmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DepartmentController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Department> department = await _context.Departments.Include(d => d.Employees).ToListAsync();
            return View(department);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDepartmentVM departmentVM)
        {
            if (!ModelState.IsValid) { return View(departmentVM); }
            bool result = await _context.Departments.AnyAsync(b => b.Name.Trim().ToLower() == departmentVM.Name.Trim().ToLower());
            if (result)
            {
                ModelState.AddModelError("Title", "A Title is available");
                return View(departmentVM);
            }

            if (departmentVM.Photo is null)
            {
                ModelState.AddModelError("Photo", "The image must be uploaded");
                return View(departmentVM);
            }
            if (!departmentVM.Photo.ValiDataType())
            {
                ModelState.AddModelError("Photo", "File Not supported");
                return View(departmentVM);
            }
            if (!departmentVM.Photo.ValiDataSize(12))
            {
                ModelState.AddModelError("Photo", "Image should not be larger than 10 mb");
                return View(departmentVM);
            }

            string fileName = await departmentVM.Photo.CreateFile(_env.WebRootPath, "img");

            Department department = new Department
            {
                Name = departmentVM.Name,
                ImgUrl = fileName,
            };

            await _context.Departments .AddAsync(department);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) { return BadRequest(); }
            Department department = await _context.Departments.FirstOrDefaultAsync(b => b.Id == id);
            if (department == null) { return NotFound(); }
            UpdateDepartmentVM departmentVM = new UpdateDepartmentVM
            {
                Name= department.Name,
                ImgUrl = department.ImgUrl
            };

            return View(departmentVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateDepartmentVM departmentVM)
        {
            if (ModelState.IsValid) { return View(departmentVM); }
            Department existed = _context.Departments.FirstOrDefault(b => b.Id == id);
            if (existed == null) { return NotFound(); }
            bool result = await _context.Departments.AnyAsync(b => b.Name.Trim().ToLower() == departmentVM.Name.Trim().ToLower() && b.Id != id);
            if (result)
            {
                ModelState.AddModelError("Title", "A Title is available");
                return View(departmentVM);
            }
            if (departmentVM.Photo is not null)
            {
                if (!departmentVM.Photo.ValiDataType())
                {
                    ModelState.AddModelError("Photo", "File Not supported");
                    return View(departmentVM);
                }
                if (!departmentVM.Photo.ValiDataSize(12))
                {
                    ModelState.AddModelError("Photo", "Image should not be larger than 10 mb");
                    return View(departmentVM);
                }
                string newImg = await departmentVM.Photo.CreateFile(_env.WebRootPath, "img");
                existed.ImgUrl.DeleteFile(_env.WebRootPath, "img");
                existed.ImgUrl = newImg;
            }
            existed.Name = departmentVM.Name;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) { return BadRequest(); }
            Department department = await _context.Departments.FirstOrDefaultAsync(b => b.Id == id);
            if (department == null) { return NotFound(); }
            department.ImgUrl.DeleteFile(_env.WebRootPath, "img");

            _context.Departments.Remove(department);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> More(int id)
        {
            if (id <= 0) { return BadRequest(); }
            Department department = await _context.Departments.Include(c => c.Employees).FirstOrDefaultAsync(c => c.Id == id);
            if (department == null) { return NotFound(); }
            return View(department);
        }
    }
}
