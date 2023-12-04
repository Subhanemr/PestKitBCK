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
            if (projectVM.MainPhoto is null)
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





            ProjectImage mainimage = new ProjectImage
            {
                IsPrimary = true,
                Url = await projectVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "img")
            };


            ProjectImage hoverimage = new ProjectImage
            {
                IsPrimary = false,
                Url = await projectVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "img")
            };

            Project project = new Project
            {
                Name = projectVM.Name,
                ProjectImages = new List<ProjectImage> { hoverimage, mainimage }
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
                    Url = await photo.CreateFileAsync(_env.WebRootPath, "img")
                });
            }

            _context.Projects.Add(project);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) { return BadRequest(); }
            Project project = await _context.Projects.Include(x => x.ProjectImages).FirstOrDefaultAsync(x => x.Id == id);
            if (project is null) { return NotFound(); }
            UpdateProjectVM projectVM = new UpdateProjectVM
            {
                Name = project.Name,
                ProjectImages = project.ProjectImages
            };
            return View(projectVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProjectVM projectVM)
        {
            Project existed = await _context.Projects.Include(pi => pi.ProjectImages).FirstOrDefaultAsync(x => x.Id == id);

            projectVM.ProjectImages = existed.ProjectImages;
            if (!ModelState.IsValid) { return View(projectVM); }
            if (existed is null) { return NotFound(); }

            if (projectVM.MainPhoto is not null)
            {
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
            }
            if (projectVM.HoverPhoto is not null)
            {
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
            }

            if (projectVM.MainPhoto is not null)
            {
                string fileName = await projectVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "img");
                ProjectImage prMain = existed.ProjectImages.FirstOrDefault(pi => pi.IsPrimary == true);

                prMain.Url.DeleteFileAsync(_env.WebRootPath, "img");
                _context.ProjectImages.Remove(prMain);

                existed.ProjectImages.Add(new ProjectImage
                {
                    IsPrimary = true,
                    Url = fileName
                });
            }

            if (projectVM.HoverPhoto is not null)
            {
                string fileName = await projectVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "img");
                ProjectImage prMain = existed.ProjectImages.FirstOrDefault(pi => pi.IsPrimary == false);

                prMain.Url.DeleteFileAsync(_env.WebRootPath, "img");
                _context.ProjectImages.Remove(prMain);

                existed.ProjectImages.Add(new ProjectImage
                {
                    IsPrimary = false,
                    Url = fileName
                });
            }

            if (existed.ProjectImages is null) { existed.ProjectImages = new List<ProjectImage>(); }

            if (projectVM.ImageIds is null) projectVM.ImageIds = new List<int>();

            List<ProjectImage> remove = existed.ProjectImages.Where(pi => pi.IsPrimary == null && !projectVM.ImageIds.Exists(imgId => imgId == pi.Id)).ToList();

            foreach (ProjectImage image in remove)
            {
                image.Url.DeleteFileAsync(_env.WebRootPath, "img");
                existed.ProjectImages.Remove(image);
            }

            if (projectVM.Photos is not null)
            {
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

                    existed.ProjectImages.Add(new ProjectImage
                    {
                        IsPrimary = null,
                        Url = await photo.CreateFileAsync(_env.WebRootPath, "img")
                    });
                }
            }

            existed.Name = projectVM.Name;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> More(int id)
        {
            if (id <= 0) { return BadRequest(); }
            Project project = await _context.Projects.Include(p => p.ProjectImages).FirstOrDefaultAsync(p => p.Id == id);
            if (project is null) { return NotFound(); }
            return View(project);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) { return BadRequest(); }
            Project project = await _context.Projects.Include(pi => pi.ProjectImages).FirstOrDefaultAsync(p => p.Id == id);
            if (project is null) { return NotFound(); };
            foreach (ProjectImage image in project.ProjectImages)
            {
                image.Url.DeleteFileAsync(_env.WebRootPath, "img");
            }

            List<ProjectImage> remove = await _context.ProjectImages.Where(p => p.ProjectId == id).ToListAsync();
            _context.ProjectImages.RemoveRange(remove);

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
