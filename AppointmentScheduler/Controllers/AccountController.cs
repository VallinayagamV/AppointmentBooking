using AppointmentScheduler.Data;
using AppointmentScheduler.Models;
using AppointmentScheduler.Models.ViewModels;
using AppointmentScheduler.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduler.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;

        public AccountController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Email);
                    HttpContext.Session.SetString("ssuserName", user.Name);
                    return RedirectToAction("Index", "Appointment");
                }
                ModelState.AddModelError("", "Invalid login attempt");
            }
            return View(model);
        }

        public async Task<IActionResult> Register()
        {
            if (!_roleManager.RoleExistsAsync(Helper.Admin).GetAwaiter().GetResult())
            {
               await _roleManager.CreateAsync(new IdentityRole(Helper.Admin));
               await _roleManager.CreateAsync(new IdentityRole(Helper.Doctor));
               await _roleManager.CreateAsync(new IdentityRole(Helper.Patient));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Name = registerViewModel.Name,
                    UserName = registerViewModel.Email,
                    Email = registerViewModel.Email
                };

                var result = await _userManager.CreateAsync(user,registerViewModel.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, registerViewModel.RoleName);
                    if (!User.IsInRole(Helper.Admin))
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                    }
                    else
                    {
                        TempData["newAdminSignUp"] = user.Name;
                    }
                    return RedirectToAction("Index", "Appointment");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(registerViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
