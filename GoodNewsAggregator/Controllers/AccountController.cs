using GoodNewsAggregator.DAL.Core.Entities;
using GoodNewsAggregator.Models.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
