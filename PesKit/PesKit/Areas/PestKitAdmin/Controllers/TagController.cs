using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKit.Areas.PestKitAdmin.ViewModels;
using PesKit.DAL;
using PesKit.Models;

namespace PesKit.Areas.PestKitAdmin.Controllers
{
    [Area("PestKitAdmin")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Tag> tags = await _context.Tags.Include(c => c.BlogTags).ThenInclude(c => c.Blog).ToListAsync();
            return View(tags);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUpdateTagVM tagVM)
        {
            if (!ModelState.IsValid) { return View(tagVM); }
            bool result = _context.Tags.Any(t => t.Name.Trim().ToLower() == tagVM.Name.Trim().ToLower());
            if (result) 
            {
                ModelState.AddModelError("Name", "A Tag is available");
                return View();
            }
            Tag tag = new Tag { Name = tagVM.Name };

            _context.Tags.Add(tag);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) { return BadRequest(); }
            Tag tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null) { return NotFound(); }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> More(int id)
        {
            if (id <= 0) { return BadRequest(); }
            Tag tag = _context.Tags.Include(c => c.BlogTags).ThenInclude(c => c.Blog).FirstOrDefault(c => c.Id == id);
            if (tag == null) { return NotFound(); }
            return View(tag);
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) { return BadRequest(); }
            Tag tag = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);
            if (tag == null) { return NotFound(); }
            CreateUpdateTagVM tagVM = new CreateUpdateTagVM { Name = tag.Name };

            return View(tagVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, CreateUpdateTagVM tagVM)
        {
            if (!ModelState.IsValid) { return View(tagVM); };
            Tag exist = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);
            if (exist == null) { return NotFound(); };
            bool result = await _context.Tags.AnyAsync(c => c.Name.Trim().ToLower() == exist.Name.Trim().ToLower() && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "A Tag is available");
                return View(exist);
            }
            exist.Name = tagVM.Name;
            await _context.SaveChangesAsync();
            return Redirect(nameof(Index));
        }
    }
}
