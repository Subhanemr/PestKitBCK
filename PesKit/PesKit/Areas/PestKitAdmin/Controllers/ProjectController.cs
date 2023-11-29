using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKit.Areas.PestKitAdmin.ViewModels;
using PesKit.DAL;
using PesKit.Models;
using PesKit.Utilities.Validata;

namespace PesKit.Areas.PestKitAdmin.Controllers
{
    [Area("PestKitAdmin")]
    public class ProjectController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProjectController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Project> projects = await _context.Projects.Include(p => p.ProjectImages).ToListAsync();
            return View(projects);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProjectVM projectVM)
        {
            if (!ModelState.IsValid) { return View(projectVM); }
            bool result = await _context.Projects.AnyAsync(p => p.Name.Trim().ToLower() == projectVM.Name.Trim().ToLower());
            if (result)
            {
                ModelState.AddModelError("Name", "A Name is available");
                return View(projectVM);
            }
            if(projectVM.MainPhoto is null)
            {
                ModelState.AddModelError("Photo", "The image must be uploaded");
                return View(projectVM);
            }
            if (!projectVM.MainPhoto.ValiDataType())
            {
                ModelState.AddModelError("Photo", "File Not supported");
                return View(projectVM);
            }
            if (!projectVM.MainPhoto.ValiDataSize(10))
            {
                ModelState.AddModelError("Photo", "Image should not be larger than 10 mb");
                return View(projectVM);
            }
            if (projectVM.HoverPhoto is null)
            {
                ModelState.AddModelError("Photo", "The image must be uploaded");
                return View(projectVM);
            }
            if (!projectVM.HoverPhoto.ValiDataType())
            {
                ModelState.AddModelError("Photo", "File Not supported");
                return View(projectVM);
            }
            if (!projectVM.HoverPhoto.ValiDataSize(10))
            {
                ModelState.AddModelError("Photo", "Image should not be larger than 10 mb");
                return View(projectVM);
            }





            ProjectImage mainimage = new ProjectImage { 
            IsPrimary = true,
            Url =await projectVM.MainPhoto.CreateFile(_env.WebRootPath, "img")
            };


            ProjectImage hoverimage = new ProjectImage
            {
                IsPrimary = false,
                Url = await projectVM.HoverPhoto.CreateFile(_env.WebRootPath, "img")
            };

            Project project = new Project {
                Name = projectVM.Name,
                ProjectImages = new List<ProjectImage>{hoverimage, mainimage}
            };

            TempData["Message"] = "";

            foreach (IFormFile photo in projectVM.Photos)
            {
                if (!photo.ValiDataType())
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.Name} type is not suitable</p>";
                    continue;
                }

                if (!photo.ValiDataSize(10))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.Name} the size is not suitable</p>";
                    continue;
                }

                project.ProjectImages.Add(new ProjectImage
                {
                    IsPrimary = null,
                    Url = await photo.CreateFile(_env.WebRootPath, "img")
                });
            }

            _context.Projects.Add(project);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

    }
}
