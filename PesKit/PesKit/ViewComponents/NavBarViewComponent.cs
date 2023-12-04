using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PesKit.DAL;
using PesKit.Models;
using PesKit.ViewModels;

namespace PesKit.ViewComponents
{
    public class NavBarViewComponent: ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _http;

        public NavBarViewComponent(AppDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<CartItemVM> cartVM = new List<CartItemVM>();
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

            return View(cartVM);
        }
    }
}
