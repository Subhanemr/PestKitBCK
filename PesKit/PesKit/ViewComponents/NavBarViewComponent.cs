using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PesKit.DAL;
using PesKit.Models;
using PesKit.ViewModels;
using System.Security.Claims;

namespace PesKit.ViewComponents
{
    public class NavBarViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly UserManager<AppUser> _userManager;

        public NavBarViewComponent(AppDbContext context, IHttpContextAccessor http, UserManager<AppUser> userManager)
        {
            _context = context;
            _http = http;
            _userManager = userManager;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<CartItemVM> cartVM = new List<CartItemVM>();
            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users.Include(b => b.BasketItems).ThenInclude(p => p.Product)
                    .FirstOrDefaultAsync(u => u.Id == _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
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
                if (_http.HttpContext.Request.Cookies["BasketPeskit"] is not null)
                {
                    List<CartCookieItemVM> cart = JsonConvert.DeserializeObject<List<CartCookieItemVM>>(_http.HttpContext.Request.Cookies["BasketPeskit"]);
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
    }
}
