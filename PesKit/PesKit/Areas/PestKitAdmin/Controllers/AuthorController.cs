using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKit.Areas.PestKitAdmin.ViewModels;
using PesKit.DAL;
using PesKit.Models;

namespace PesKit.Areas.PestKitAdmin.Controllers
{
    [Area("PestKitAdmin")]
    public class AuthorController : Controller
    {
        private readonly AppDbContext _context;

        public AuthorController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Author> authors = await _context.Author.Include(a => a.Blogs).ToListAsync();
            return View(authors);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUpdateAuthorVM authorVM)
        {
            if (!ModelState.IsValid) return View();

            bool result = await _context.Author.AnyAsync(c => c.Name.ToLower().Trim() == authorVM.Name.ToLower().Trim());

            if (result)
            {
                ModelState.AddModelError("Name", "A Category is available");
                return View();
            }
            Author author = new Author 
            { 
                Name = authorVM.Name,
                Surname=authorVM.Surname
            };
            await _context.Author.AddAsync(author);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if(id <= 0) { return BadRequest(); }
            Author author = await _context.Author.FirstOrDefaultAsync(c => c.Id == id);
            if (author == null) { return NotFound(); }
            CreateUpdateAuthorVM authorVM = new CreateUpdateAuthorVM {Name= author.Name, Surname=author.Surname };

            return View(authorVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, CreateUpdateAuthorVM authorVM)
        {
            if (!ModelState.IsValid) { return View(authorVM); };
            Author exist = await _context.Author.FirstOrDefaultAsync(c => c.Id == id);
            if (exist == null) { return NotFound(); };
            bool result = await _context.Author.AnyAsync(c => c.Name.Trim().ToLower() == exist.Name.Trim().ToLower() && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "A Name is available");
                return View(exist);
            }
            exist.Name = authorVM.Name;
            exist.Surname = authorVM.Surname;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if(id < 0) { return BadRequest(); }
            
            Author author = await _context.Author.FirstOrDefaultAsync(c => c.Id == id);
            if(author == null) { return NotFound(); }
            _context.Author.Remove(author); 
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> More(int id)
        {
            if (id <= 0) { return BadRequest(); }
            Author author = _context.Author.Include(c=> c.Blogs).FirstOrDefault(c => c.Id == id);
            if (author == null) { return NotFound(); }
            return View(author);
        }

    }
}
