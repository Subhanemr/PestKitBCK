using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PesKit.DAL;
using PesKit.Models;
using PesKit.ViewModels;

namespace PesKit.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<CartItemVM> cartVM = new List<CartItemVM>();
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
                return View(cartVM);
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            List<CartCookieItemVM> cart;
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
            return RedirectToAction(nameof(Index), "Home");
        }
        public async Task<IActionResult> DeleteItem(int id)
        {
            if (id <= 0) return BadRequest();
            List<CartCookieItemVM> cart = JsonConvert.DeserializeObject<List<CartCookieItemVM>>(Request.Cookies["BasketPeskit"]);
            CartCookieItemVM item = cart.FirstOrDefault(c => c.Id == id);
            if (item == null) return NotFound();

            cart.Remove(item);


            string json = JsonConvert.SerializeObject(cart);
            Response.Cookies.Append("BasketPeskit", json, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(1)
            });

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CountMinus(int id)
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

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> CountPlus(int id)
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

            return RedirectToAction(nameof(Index));
        }
    }
}
