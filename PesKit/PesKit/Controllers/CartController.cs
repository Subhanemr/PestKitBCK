using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PesKit.DAL;
using PesKit.Interfaces;
using PesKit.LayoutService;
using PesKit.Models;
using PesKit.ViewModels;
using System.Security.Claims;

namespace PesKit.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ServicesLayout _layoutServices;
        private readonly IEmailService _emailService;

        public CartController(AppDbContext context, UserManager<AppUser> userManager, ServicesLayout layoutServices, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _layoutServices = layoutServices;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            List<CartItemVM> cartVM = new List<CartItemVM>();

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users.Include(b => b.BasketItems.Where(p => p.OrderId == null)).ThenInclude(p => p.Product)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                foreach (BasketItem item in appUser.BasketItems)
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
            List<CartItemVM> cartItems;

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users.Include(p => p.BasketItems.Where(p => p.OrderId == null)).ThenInclude(p => p.Product)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
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

                cartItems = await _layoutServices.GetDbItemAsync(appUser);
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

                cartItems = await _layoutServices.GetCookieItemAsync(cart);
            }
            return PartialView("CartOfcanvas/_CartOffcanvasPartialView", cartItems);
        }

        public async Task<IActionResult> DeleteItem(int id)
        {
            if (id <= 0) return BadRequest();
            List<CartItemVM> cartItems;
            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(b => b.BasketItems.Where(p => p.OrderId == null)).ThenInclude(p => p.Product)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (appUser == null) return NotFound();
                BasketItem item = appUser.BasketItems.FirstOrDefault(b => b.ProductId == id);
                if (item == null) return BadRequest();

                _context.BasketItems.Remove(item);
                await _context.SaveChangesAsync();
                cartItems = await _layoutServices.GetDbItemAsync(appUser);
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
                cartItems = await _layoutServices.GetCookieItemAsync(cart);
            }

            return PartialView("CartOfcanvas/_CartOffcanvasPartialView", cartItems);
        }

        public async Task<IActionResult> CountMinus(int id)
        {

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(b => b.BasketItems.Where(p => p.OrderId == null))
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
                    .Include(b => b.BasketItems.Where(p => p.OrderId == null))
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

        public async Task<IActionResult> Checkout()
        {
            AppUser appUser = await _userManager.Users.Include(b => b.BasketItems.Where(p => p.OrderId == null)).ThenInclude(bi => bi.Product)
                .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            OrderVM orderVM = new OrderVM
            {
                BasketItems = appUser.BasketItems
            };

            return View(orderVM);
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(OrderVM orderVM)
        {
            AppUser appUser = await _userManager.Users.Include(b => b.BasketItems.Where(p => p.OrderId == null)).ThenInclude(bi => bi.Product)
                .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!appUser.BasketItems.Any())
            {
                orderVM.BasketItems = appUser.BasketItems;
                return View(orderVM);
            }
            if (!ModelState.IsValid)
            {
                orderVM.BasketItems = appUser.BasketItems;
                return View(orderVM);
            }
            decimal total = 0;
            foreach (BasketItem item in appUser.BasketItems)
            {
                item.Price = item.Product.Price;
                total += item.Price * item.Count;
            }

            Order order = new Order
            {
                Status = null,
                Address = orderVM.Address,
                PruchaseAt = DateTime.Now,
                AppUserId = appUser.Id,
                BasketItems = appUser.BasketItems,
                TotalPrice = total
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            string bodyHtml = await Table(order);

            await _emailService.SendEmail(appUser.Email, "Your Order", bodyHtml, true);

            return RedirectToAction("MyOrders", "Account");
        }
        public async Task<string> Table(Order order)
        {
            string bodyHtml = @"
<style>
table {font - family: arial, sans-serif;
  border-collapse: collapse;
  width: 100%;
  border-radius: 10px;
  background-color: #E2E0E0;
}

td, th {border - bottom: 1px solid #9E9E9E;
  text-align: left;
  border-radius: 25px;
  padding: 20px;

            }
th{
font-weight: bold;
}


</style>

<h2>Your Order</h2>

<table>
                            <thead>
                                <tr>
                                    <th>Product</th>
                                    <th>Unit Price</th>
                                    <th>Quantity</th>
                                    <th>Total</th>
                                </tr>
                            </thead>
                            <tbody>";

            decimal total = 0;
            foreach (BasketItem item in order.BasketItems)
            {
                total += item.Price * item.Count;
                bodyHtml += $@"
                                    <tr>

                                        <td>
                                            <a>{item.Product.Name}</a>
                                        </td>
                                        <td>
                                            <p>{item.Price}</p>
                                        </td>
                                        <td>
                                            <div>
                                                <p>{item.Count}</p>
                                            </div>
                                        </td>
                                        <td>
                                            <span>{total}</span>
                                        </td>
                                    </tr>
                                    <tr>";
            }

            bodyHtml += $@"                              
                                    	<td colspan=""4"">
                                    		<h2>Orders totals</h2> <p><h3>Total</h4> <h4>${total}</h5><p>
                                        </td>
                                    </tr>

                            </tbody>
</table>";


            return bodyHtml;
        }
    }
}
