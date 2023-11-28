
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
            CreateBlogVM blogVM = new CreateBlogVM
            {
                Authors = await _context.Author.ToListAsync(),
                Tags = await _context.Tags.ToListAsync()
            };
            return View(blogVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBlogVM blogVM)
        {
            if (!ModelState.IsValid)
            {
                blogVM.Authors = await _context.Author.ToListAsync();
                blogVM.Tags = await _context.Tags.ToListAsync();
                return View(blogVM);
            }
            bool result = await _context.Blogs.AnyAsync(b => b.Title.Trim().ToLower() == blogVM.Title.Trim().ToLower());
            if (result)
            {
                blogVM.Authors = await _context.Author.ToListAsync();
                blogVM.Tags = await _context.Tags.ToListAsync();
                ModelState.AddModelError("Title", "A Title is available");
                return View(blogVM);
            }

            if (blogVM.Photo is null)
            {
                blogVM.Authors = await _context.Author.ToListAsync();
                blogVM.Tags = await _context.Tags.ToListAsync();
                ModelState.AddModelError("Photo", "The image must be uploaded");
                return View(blogVM);
            }
            if (!blogVM.Photo.ValiDataType())
            {
                blogVM.Authors = await _context.Author.ToListAsync();
                blogVM.Tags = await _context.Tags.ToListAsync();
                ModelState.AddModelError("Photo", "File Not supported");
                return View(blogVM);
            }
            if (!blogVM.Photo.ValiDataSize(12))
            {
                blogVM.Authors = await _context.Author.ToListAsync();
                blogVM.Tags = await _context.Tags.ToListAsync();
                ModelState.AddModelError("Photo", "Image should not be larger than 10 mb");
                return View(blogVM);
            }

            foreach (int tagId in blogVM.TagIds)
            {
                bool tagResult = await _context.Tags.AnyAsync(t => t.Id == tagId);
                if (!tagResult)
                {
                    blogVM.Authors = await _context.Author.ToListAsync();
                    blogVM.Tags = await _context.Tags.ToListAsync();
                    return View(blogVM);
                }
            }

            string fileName = await blogVM.Photo.CreateFile(_env.WebRootPath, "img");

            Blog blog = new Blog
            {
                Title = blogVM.Title,
                Description = blogVM.Description,
                DateTime = DateTime.Now,
                ImgUrl = fileName,
                AuthorId = (int)blogVM.AuthorId,
                CommentCount = blogVM.CommentCount,
                Tags = blogVM.TagIds.Select(tagId => new BlogTag { TagId = tagId }).ToList(),
            };

            await _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) { return BadRequest(); }
            Blog blog = await _context.Blogs.Include(c=>c.Tags).FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null) 
                return NotFound(); 
            UpdateBlogVM blogVM = new UpdateBlogVM
            {
                Title = blog.Title,
                Description = blog.Description,
                ImgUrl = blog.ImgUrl,
                AuthorId = blog.AuthorId,
                CommentCount = blog.CommentCount,
                Authors = await _context.Author.ToListAsync(),
                Tagss = await _context.Tags.ToListAsync(),
                TagIds = blog.Tags.Select(p => p.TagId).ToList(),
            };

            return View(blogVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateBlogVM blogVM)
        {
            if (!ModelState.IsValid)
            {
                blogVM.Authors = await _context.Author.ToListAsync();
                blogVM.Tagss = await _context.Tags.ToListAsync();
                return View(blogVM);
            }
            Blog existed = _context.Blogs.Include(c => c.Tags).FirstOrDefault(b => b.Id == id);
            if (existed == null) { return NotFound(); }
            bool result = await _context.Blogs.AnyAsync(b => b.Title.Trim().ToLower() == blogVM.Title.Trim().ToLower() && b.Id != id);
            if (result)
            {
                blogVM.Authors = await _context.Author.ToListAsync();
                blogVM.Tagss = await _context.Tags.ToListAsync();
                ModelState.AddModelError("Title", "A Title is available");
                return View(blogVM);
            }
            if (blogVM.Photo is not null)
            {
                if (!blogVM.Photo.ValiDataType())
                {
                    blogVM.Tagss = await _context.Tags.ToListAsync();
                    blogVM.Authors = await _context.Author.ToListAsync();
                    ModelState.AddModelError("Photo", "File Not supported");
                    return View(blogVM);
                }
                if (!blogVM.Photo.ValiDataSize(12))
                {
                    blogVM.Tagss = await _context.Tags.ToListAsync();
                    blogVM.Authors = await _context.Author.ToListAsync();
                    ModelState.AddModelError("Photo", "Image should not be larger than 10 mb");
                    return View(blogVM);
                }
                string newImg = await blogVM.Photo.CreateFile(_env.WebRootPath, "img");
                existed.ImgUrl.DeleteFile(_env.WebRootPath, "img");
                existed.ImgUrl = newImg;
            }

            List<BlogTag> colorToRemove = existed.Tags
    .Where(BlogTag => !blogVM.TagIds.Contains(BlogTag.TagId))
    .ToList();
            _context.BlogTags.RemoveRange(colorToRemove);

            List<BlogTag> colorToAdd = blogVM.TagIds
                .Except(existed.Tags.Select(pc => pc.TagId))
                .Select(tagId => new BlogTag { TagId = tagId })
                .ToList();
            existed.Tags.AddRange(colorToAdd);

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
            blog.ImgUrl.DeleteFile(_env.WebRootPath, "img");


            _context.Blogs.Remove(blog);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> More(int id)
        {
            if (id <= 0) { return BadRequest(); }
            Blog blog = await _context.Blogs.Include(c => c.Author).Include(c=> c.Tags).ThenInclude(c => c.Tag).FirstOrDefaultAsync(c => c.Id == id);
            if (blog == null) { return NotFound(); }
            return View(blog);
        }

    }
}
