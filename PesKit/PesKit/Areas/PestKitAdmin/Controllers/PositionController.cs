using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKit.Areas.PestKitAdmin.ViewModels;
using PesKit.DAL;
using PesKit.Models;

namespace PesKit.Areas.PestKitAdmin.Controllers
{
    [Area("PestKitAdmin")]
    public class PositionController : Controller
    {
        private readonly AppDbContext _context;

        public PositionController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Position> list = await _context.Positions.Include(p => p.Employees).ToListAsync();
            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUpdatePositionVM positionVM)
        {
            if (!ModelState.IsValid) return View(positionVM);

            bool result = await _context.Positions.AnyAsync(c => c.Name.ToLower().Trim() == positionVM.Name.ToLower().Trim());

            if (result)
            {
                ModelState.AddModelError("Name", "A Category is available");
                return View(positionVM);
            }
            Position category = new Position { Name = positionVM.Name };
            await _context.Positions.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Position position = await _context.Positions.FirstOrDefaultAsync(c => c.Id == id);

            if (position == null) return NotFound();
            CreateUpdatePositionVM positionVM = new CreateUpdatePositionVM { Name = position.Name };

            return View(positionVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, CreateUpdatePositionVM positionVM)
        {
            if (!ModelState.IsValid) return View(positionVM);

            Position existed = await _context.Positions.FirstOrDefaultAsync(c => c.Id == id);
            if (existed == null) return NotFound();

            bool result = await _context.Positions.AnyAsync(c => c.Name.ToLower().Trim() == positionVM.Name.ToLower().Trim() && c.Id != id);

            if (result)
            {
                ModelState.AddModelError("Name", "A Category is available");
                return View(positionVM);
            }

            existed.Name = positionVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Position existed = await _context.Positions.FirstOrDefaultAsync(c => c.Id == id);
            if (existed == null) return NotFound();
            _context.Positions.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> More(int id)
        {
            if (id <= 0) return BadRequest();
            Position position = await _context.Positions
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (position == null) return NotFound();

            return View(position);
        }
    }
}
