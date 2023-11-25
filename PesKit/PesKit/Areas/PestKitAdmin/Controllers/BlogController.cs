using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKit.Areas.PestKitAdmin.ViewModels;
using PesKit.DAL;
using PesKit.Models;
using PesKit.Utilities.Validata;

namespace PesKit.Areas.PestKitAdmin.Controllers
{
    [Area("PestKitAdmin")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BlogController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Blog> blogs = await _context.Blogs.Include(b => b.Author).ToListAsync();
            return View(blogs);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Authors = await _context.Author.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBlogVM blogVM)
        {
            bool result = await _context.Blogs.AnyAsync(b => b.Title.Trim().ToLower() == blogVM.Title.Trim().ToLower());
            if (result)
            {
                ViewBag.Authors = await _context.Author.ToListAsync();
                ModelState.AddModelError("Title", "A Title is available");
                return View(blogVM);
            }

            if (blogVM.Photo is null)
            {
                ViewBag.Authors = await _context.Author.ToListAsync();
                ModelState.AddModelError("Photo", "The image must be uploaded");
                return View(blogVM);
            }
            if (!blogVM.Photo.ValiDataType())
            {
                ViewBag.Authors = await _context.Author.ToListAsync();
                ModelState.AddModelError("Photo", "File Not supported");
                return View(blogVM);
            }
            if (!blogVM.Photo.ValiDataSize(12))
            {
                ViewBag.Authors = await _context.Author.ToListAsync();
                ModelState.AddModelError("Photo", "Image should not be larger than 10 mb");
                return View(blogVM);
            }

            string fileName = await blogVM.Photo.CreateFile(_env.WebRootPath, "img");

            Blog blog = new Blog
            {
                Title = blogVM.Title,
                Description = blogVM.Description,
                DateTime = DateTime.Now,
                ImgUrl = fileName,
                AuthorId = (int)blogVM.AuthorId,
                CommentCount = blogVM.CommentCount
            };

            await _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) { return BadRequest(); }
            Blog blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null) { return NotFound(); }
            UpdateBlogVM blogVM = new UpdateBlogVM 
            { 
                Title=blog.Title,
                Description = blog.Description,
                ImgUrl = blog.ImgUrl,
                AuthorId=blog.AuthorId,
                CommentCount = blog.CommentCount
            };
            ViewBag.Authors = await _context.Author.ToListAsync();

            return View(blogVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateBlogVM blogVM)
        {
            if (ModelState.IsValid) { return View(blogVM); }
            Blog existed = _context.Blogs.FirstOrDefault(b => b.Id == id);
            if (existed == null) { return NotFound(); }
            if (blogVM.Photo is not null)
            {
                if (!blogVM.Photo.ValiDataType())
                {
                    ViewBag.Authors = await _context.Author.ToListAsync();
                    ModelState.AddModelError("Photo", "File Not supported");
                    return View(blogVM);
                }
                if (!blogVM.Photo.ValiDataSize(12))
                {
                    ViewBag.Authors = await _context.Author.ToListAsync();
                    ModelState.AddModelError("Photo", "Image should not be larger than 10 mb");
                    return View(blogVM);
                }
                string newImg = await blogVM.Photo.CreateFile(_env.WebRootPath, "img");
                existed.ImgUrl.DeleteFile(_env.WebRootPath, "img");
                existed.ImgUrl = newImg;
            }
            existed.Title = blogVM.Title;
            existed.Description = blogVM.Description;
            existed.AuthorId = (int)blogVM.AuthorId;
            existed.CommentCount = (int)blogVM.CommentCount;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) { return BadRequest(); }
            Blog blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null) { return NotFound(); }

            _context.Blogs.Remove(blog);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> More(int id)
        {
            if (id <= 0) { return BadRequest(); }
            Blog blog = await _context.Blogs.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == id);
            if (blog == null) { return NotFound(); }
            return View(blog);
        }

    }
}
