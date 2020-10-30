using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManager.Mvc.Models;
using EmployeeManager.Mvc.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

namespace EmployeeManager.Mvc.Controllers
{
    public class SecurityController : Controller
    {
        private readonly UserManager<AppIdentityUser> userManager;
        private readonly RoleManager<AppIdentityRole> roleManager;
        private readonly SignInManager<AppIdentityUser> signInManager;

        public SecurityController(UserManager<AppIdentityUser> userManager,
            RoleManager<AppIdentityRole> roleManager,
            SignInManager<AppIdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Register obj)
        {
            if (ModelState.IsValid)
            {
                if (!roleManager.RoleExistsAsync("Manager").Result) 
                {
                    AppIdentityRole role = new AppIdentityRole();
                    role.Name = "Manager";
                    role.Description = "Can perform CRUD operations.";
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                }

                AppIdentityUser user = new AppIdentityUser();
                user.UserName = obj.UserName;
                user.Email = obj.Email;
                user.FullName = obj.FullName;
                user.BirthDate = obj.BirthDate;

                IdentityResult result = userManager.CreateAsync(user, obj.Password).Result;

                if (result.Succeeded) 
                {
                    userManager.AddToRoleAsync(user, "Manager").Wait();
                    return RedirectToAction("SignIn", "Security");
                }
                else 
                {
                    ModelState.AddModelError("", "Invalid user details!");
                }

            }
            return View(obj);
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignIn(SignIn obj)
        {
            if (ModelState.IsValid) 
            {
                var result = signInManager.PasswordSignInAsync
                    (obj.UserName, obj.Password, obj.RememberMe, false).Result;

                if (result.Succeeded) 
                {
                    return RedirectToAction("List", "EmployeerManager");
                }
                else
                {
                    ModelState.AddModelError("", " Invalid user details!");
                }

            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult SignOut()
        {
            signInManager.SignOutAsync().Wait();
            return RedirectToAction("SignIn", "Security");
        }
    }
}
