using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PesKit.Models;
using PesKit.Utilities.Enums;
using PesKit.Utilities.Validata;
using PesKit.ViewModels;
using System.Text.RegularExpressions;

namespace PesKit.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _env = env;
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
            await _signInManager.SignInAsync(appUser, false);
            if (returnUrl == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(returnUrl);
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
    }
}
