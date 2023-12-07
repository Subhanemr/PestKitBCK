using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PesKit.DAL;
using PesKit.Models;
using PesKit.ViewModels;
using System.Security.Claims;

namespace PesKit.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CartController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<CartItemVM> cartVM = new List<CartItemVM>();

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users.Include(b => b.BasketItems).ThenInclude(p => p.Product)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                foreach(BasketItem item in appUser.BasketItems)
                {
                    cartVM.Add(new CartItemVM
                    {
                        Id = item.ProductId,
                        Name = item.Product.Name,
                        Price = item.Product.Price,
                        Count = item.Count,
                        SubTotal = item.Count * item.Product.Price,
                        Img = item.Product.Img
                    });
                }
            }
            else
            {
                if (Request.Cookies["BasketPeskit"] is not null)
                {
                    List<CartCookieItemVM> cart = JsonConvert.DeserializeObject<List<CartCookieItemVM>>(Request.Cookies["BasketPeskit"]);
                    foreach (CartCookieItemVM cartCookieItemVM in cart)
                    {
                        Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == cartCookieItemVM.Id);
                        if (product is not null)
                        {
                            CartItemVM cartItemVM = new CartItemVM
                            {
                                Id = cartCookieItemVM.Id,
                                Name = product.Name,
                                Price = product.Price,
                                Img = product.Img,
                                Count = cartCookieItemVM.Count,
                                SubTotal = (decimal)cartCookieItemVM.Count * product.Price

                            };
                            cartVM.Add(cartItemVM);
                        }
                    }
                }
            }
            return View(cartVM);
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            List<CartCookieItemVM> cart;

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users.Include(p => p.BasketItems).FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (appUser == null) return NotFound();
                BasketItem item = appUser.BasketItems.FirstOrDefault(b => b.ProductId == id);
                if (item == null)
                {
                    item = new BasketItem
                    {
                        AppUserId = appUser.Id,
                        ProductId = product.Id,
                        Price = product.Price,
                        Count = 1
                    };
                    appUser.BasketItems.Add(item);
                }
                else
                {
                    item.Count++;
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                if (Request.Cookies["BasketPeskit"] is not null)
                {
                    cart = JsonConvert.DeserializeObject<List<CartCookieItemVM>>(Request.Cookies["BasketPeskit"]);
                    CartCookieItemVM item = cart.FirstOrDefault(c => c.Id == id);
                    if (item == null)
                    {
                        CartCookieItemVM cartCookieItem = new CartCookieItemVM
                        {
                            Id = id,
                            Count = 1
                        };
                        cart.Add(cartCookieItem);
                    }
                    else
                    {
                        item.Count++;
                    }
                }
                else
                {
                    cart = new List<CartCookieItemVM>();
                    CartCookieItemVM cartCookieItem = new CartCookieItemVM { Id = id, Count = 1 };
                    cart.Add(cartCookieItem);
                }
                string json = JsonConvert.SerializeObject(cart);
                Response.Cookies.Append("BasketPeskit", json, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(1)
                });

            }
            return RedirectToAction(nameof(Index), "Home");
        }
        public async Task<IActionResult> DeleteItem(int id)
        {
            if (id <= 0) return BadRequest();
            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(b => b.BasketItems)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (appUser == null) return NotFound();
                BasketItem item = appUser.BasketItems.FirstOrDefault(b => b.ProductId == id);
                if (item == null) return BadRequest();

                _context.BasketItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            else
            {
                List<CartCookieItemVM> cart = JsonConvert.DeserializeObject<List<CartCookieItemVM>>(Request.Cookies["BasketPeskit"]);
                CartCookieItemVM item = cart.FirstOrDefault(c => c.Id == id);
                if (item == null) return NotFound();

                cart.Remove(item);


                string json = JsonConvert.SerializeObject(cart);
                Response.Cookies.Append("BasketPeskit", json, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(1)
                });
            }


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CountMinus(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(b => b.BasketItems)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (appUser == null) return NotFound();

                BasketItem item = appUser.BasketItems.FirstOrDefault(b => b.ProductId == id);

                if (item == null) return BadRequest();

                item.Count--;
                if (item.Count <= 0)
                {
                    return await DeleteItem(id);
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                List<CartCookieItemVM> cart = JsonConvert.DeserializeObject<List<CartCookieItemVM>>(Request.Cookies["BasketPeskit"]);

                CartCookieItemVM item = cart.FirstOrDefault(c => c.Id == id);

                if (item == null) return BadRequest();

                item.Count--;
                if (item.Count <= 0)
                {
                    return await DeleteItem(id);
                }

                string json = JsonConvert.SerializeObject(cart);
                Response.Cookies.Append("BasketPeskit", json, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(1)
                });
            }


            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> CountPlus(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(b => b.BasketItems)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (appUser == null) return NotFound();

                BasketItem item = appUser.BasketItems.FirstOrDefault(b => b.ProductId == id);

                if (item == null) return BadRequest();

                item.Count++;
                if (item.Count <= 0)
                {
                    return await DeleteItem(id);
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                List<CartCookieItemVM> cart = JsonConvert.DeserializeObject<List<CartCookieItemVM>>(Request.Cookies["BasketPeskit"]);

                CartCookieItemVM item = cart.FirstOrDefault(c => c.Id == id);

                if (item == null) return BadRequest();

                item.Count++;
                if (item.Count <= 0)
                {
                    return await DeleteItem(id);
                }

                string json = JsonConvert.SerializeObject(cart);
                Response.Cookies.Append("BasketPeskit", json, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(1)
                });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
