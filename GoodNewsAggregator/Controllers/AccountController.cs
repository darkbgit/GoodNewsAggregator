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
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace GoodNewsAggregator.Controllers
{

    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManeger;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;

        public AccountController(UserManager<User> userManeger,
            SignInManager<User> signInManager,
            IMapper mapper)
        {
            _userManeger = userManeger;
            _signInManager = signInManager;
            _mapper = mapper;
        }


        [HttpGet]
        public IActionResult Register() => View();


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                User user = _mapper.Map<User>(model);
                //    new User
                //{
                //    Email = model.Email,
                //    UserName = model.Email,
                //    Year = model.Year
                //};

                var result = await _userManeger.CreateAsync(user, model.Password);

                if(result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    Log.Information($"User {user.UserName} register");
                    return RedirectToAction("Index", "News");
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
                {
                    Log.Information($"User {model.Email} login");
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        //return Redirect(model.ReturnUrl);
                        return Json(new { result = "Redirect", url = model.ReturnUrl });
                    }
                    else
                    {
                        return Json(new { result = "Redirect", url = Url.Action("Index", "News") });
                        //return RedirectToAction("Index", "Home");
                    }
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
            Log.Information("User logged out");
            return RedirectToAction("Index", "News");
        }
    }
}
