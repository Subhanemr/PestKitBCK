using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using PesKit.Interfaces;
using PesKit.Models;
using PesKit.Utilities.Enums;
using PesKit.Utilities.Validata;
using PesKit.ViewModels;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace PesKit.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment env, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _env = env;
            _emailService = emailService;
        }


        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM, string? returnUrl)
        {
            if (!ModelState.IsValid) return View(registerVM);
            if (!Regex.IsMatch(registerVM.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                ModelState.AddModelError("Email", "Invalid email format");
                return View(registerVM);
            }
            AppUser appUser = new AppUser
            {
                Name = registerVM.Name.Capitalize(),
                Email = registerVM.Email,
                Surname = registerVM.Surname.Capitalize(),
                UserName = registerVM.UserName
            };

            IdentityResult result = await _userManager.CreateAsync(appUser, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(registerVM);
            }

            await _userManager.AddToRoleAsync(appUser, UserRoles.Member.ToString());


            var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, Email = appUser.Email }, Request.Scheme);
            await _emailService.SendEmail(appUser.Email, "Email Confirmation", confirmationLink);
            
            Response.Cookies.Delete("BasketPeskit");

            //await _signInManager.SignInAsync(appUser, false);

            //if (returnUrl == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            //return Redirect(returnUrl);
            return RedirectToAction(nameof(SuccessfullyRegistred), "Account");

        }

        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            AppUser appUser = await _userManager.FindByEmailAsync(email);
            if (appUser == null) return NotFound();
            var result = await _userManager.ConfirmEmailAsync(appUser, token);
            if (!result.Succeeded)
            {
                return BadRequest();
            }
            await _signInManager.SignInAsync(appUser, false);

            return View();
        }

        public IActionResult SuccessfullyRegistred()
        {
            return View();
        }

        public async Task<IActionResult> LogOut(string? returnUrl)
        {
            await _signInManager.SignOutAsync();
            if (returnUrl == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(returnUrl);
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM, string? returnUrl)
        {
            if (!ModelState.IsValid) return View(loginVM);
            AppUser user = await _userManager.FindByNameAsync(loginVM.UserNameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail);
                if(user == null)
                {
                    ModelState.AddModelError(string.Empty, "Username, Email or Password is incorrect");
                    return View(loginVM);
                }
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.IsRemembered, true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Login is not enable please try latter");
                return View(loginVM);
            }
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Confirm your email");
                return View(loginVM);
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Username, Email or Password is incorrect");
                return View(loginVM);
            }

            Response.Cookies.Delete("BasketPeskit");
            if (returnUrl == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(returnUrl);
        }
        public async Task<IActionResult> CreateRoles()
        {
            foreach (var role in Enum.GetValues(typeof(UserRoles)))
            {
                if (!(await _roleManager.RoleExistsAsync(role.ToString())))
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = role.ToString()
                    });
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> EditUser()
        {
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            EditUserVM editUserVM = new EditUserVM
            {
                Name = appUser.Name,
                Surname = appUser.Surname,
                UserName = appUser.Name,
                Img = appUser.Img,
            };
            return View(editUserVM);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserVM editUserVM)
        {
            if (!ModelState.IsValid) return View(editUserVM);
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (appUser == null) return View(editUserVM);
            appUser.Name = editUserVM.Name.Capitalize();
            appUser.Surname = editUserVM.Surname.Capitalize();
            appUser.UserName = editUserVM.UserName;
            if (editUserVM.Photo != null)
            {
                if (!editUserVM.Photo.ValiDataType())
                {
                    ModelState.AddModelError("Photo", "File Not supported");
                    return View(editUserVM);
                }

                if (!editUserVM.Photo.ValiDataSize(10))
                {
                    ModelState.AddModelError("Photo", "Image should not be larger than 10 mb");
                    return View(editUserVM);
                }
                string fileName = await editUserVM.Photo.CreateFileAsync(_env.WebRootPath, "img");
                appUser.Img.DeleteFileAsync(_env.WebRootPath, "img");
                appUser.Img = fileName;
            }

            await _userManager.UpdateAsync(appUser);
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(appUser, isPersistent: false);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> MyOrders()
        {
            AppUser appUser = await _userManager.Users
                .Include(b => b.BasketItems.Where(bi => bi.OrderId != null))
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

            List<CartItemVM> cartVM = new List<CartItemVM>();

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
            return View(cartVM);
        }
    }
}
