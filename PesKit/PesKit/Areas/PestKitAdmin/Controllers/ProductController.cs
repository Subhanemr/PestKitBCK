using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKit.Areas.PestKitAdmin.ViewModels;
using PesKit.DAL;
using PesKit.Models;
using PesKit.Utilities.Exceptions;
using PesKit.Utilities.Validata;

namespace PesKit.Areas.PestKitAdmin.Controllers
{
    [Area("PestKitAdmin")]
    [Authorize(Roles = "Admin,Moderator")]
    [AutoValidateAntiforgeryToken]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products.ToListAsync();
            return View(products);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            if (!ModelState.IsValid) return View(productVM);
            bool result = await _context.Products.AnyAsync(p => p.Name.Trim().ToLower() == productVM.Name.Trim().ToLower());
            if (result)
            {
                ModelState.AddModelError("Name", "A Category is available");
                return View(productVM);
            }
            if (productVM.Photo is null)
            {
                ModelState.AddModelError("Photo", "A Image is available");
                return View(productVM);

            }
            if (!productVM.Photo.ValiDataType())
            {
                ModelState.AddModelError("Photo", "File Not supported");
                return View(productVM);
            }
            if (!productVM.Photo.ValiDataSize(12))
            {
                ModelState.AddModelError("Photo", "Image should not be larger than 10 mb");
                return View(productVM);
            }
            Product product = new Product
            {
                Name = productVM.Name,
                Price = productVM.Price,
                Description = productVM.Description,
                Img = await productVM.Photo.CreateFileAsync(_env.WebRootPath, "img")
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) throw new WrongRequestException("The request sent does not exist");
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) throw new NotFoundException("Your request was not found");
            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Img = product.Img
            };
            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {
            if (!ModelState.IsValid) return View(productVM);
            bool result = await _context.Products.AnyAsync(p => p.Name.Trim().ToLower() == productVM.Name.Trim().ToLower() && p.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "A Category is available");
                return View(productVM);
            }
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (productVM.Photo != null)
            {
                if (!productVM.Photo.ValiDataType())
                {
                    ModelState.AddModelError("Photo", "File Not supported");
                    return View(productVM);
                }
                if (!productVM.Photo.ValiDataSize(12))
                {
                    ModelState.AddModelError("Photo", "Image should not be larger than 10 mb");
                    return View(productVM);
                }
                string fileName = await productVM.Photo.CreateFileAsync(_env.WebRootPath, "img");
                product.Img.DeleteFileAsync(_env.WebRootPath, "img");
                product.Img = fileName;
            }

            product.Name = productVM.Name;
            product.Description = productVM.Description;
            product.Price = productVM.Price;
            product.Img = productVM.Img;


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) { throw new WrongRequestException("The request sent does not exist"); }

            Product product = await _context.Products.SingleOrDefaultAsync(e => e.Id == id);
            if (product == null) { throw new NotFoundException("Your request was not found"); }
            product.Img.DeleteFileAsync(_env.WebRootPath, "img");


            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> More(int id)
        {
            if (id <= 0) { throw new WrongRequestException("The request sent does not exist"); };
            Product product = await _context.Products.FirstOrDefaultAsync(e => e.Id == id);

            if (product == null) { throw new NotFoundException("Your request was not found"); };

            return View(product);
        }
    }
}
