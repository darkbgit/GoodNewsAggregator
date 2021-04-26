using GoodNewsAggregator.DAL.Core.Entities;
using GoodNewsAggregator.Models.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GoodNewsAggregator.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManeger;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManeger,
            SignInManager<User> signInManager)
        {
            _userManeger = userManeger;
            _signInManager = signInManager;
        }

        

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                User user = new User
                {
                    Email = model.Email,
                    UserName = model.Email,
                };

                var result = await _userManeger.CreateAsync(user, model.Password);

                if(result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager
                    .PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        //return Redirect(model.ReturnUrl);
                        return Json(new { result = "Redirect", url = model.ReturnUrl });
                    }
                    else
                    {
                        return Json(new {result = "Redirect", url = Url.Action("Index", "Home")});
                        //return RedirectToAction("Index", "Home");
                    }
                else
                {
                    ModelState.AddModelError("", "Неверный логин и (или) пароль");
                }
            }

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
