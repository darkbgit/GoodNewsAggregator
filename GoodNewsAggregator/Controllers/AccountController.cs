using AutoMapper;
using GoodNewsAggregator.DAL.Core.Entities;
using GoodNewsAggregator.Models.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Controllers
{

    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }


        public async Task<IActionResult> UserCabinet(string name)
        {
            if (User.Identity?.Name == name)
            {
                var user = await _userManager.FindByNameAsync(name);
                var model = _mapper.Map<UserCabinetViewModel>(user);
                return View(model);
            }
            return NotFound();
        }

        public IActionResult AdminCabinet(string name)
        {
            if (User.Identity?.Name == name)
            {
                return View();
            }

            return NotFound();
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

                var result = await _userManager.CreateAsync(user, model.Password);

                if(result.Succeeded)
                {
                    Log.Information($"User {user.UserName} register");

                    var roleResult = await _userManager.AddToRoleAsync(user, "user");
                    if (roleResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        Log.Information($"User {user.UserName} added role \"user\"");
                        return RedirectToAction("Index", "News");
                    }
                    else
                    {
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
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
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager
                .PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                Log.Information($"User {model.Email} login");
                return !string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)
                    ? Json(new {result = "Redirect", url = model.ReturnUrl})
                    : Json(new {result = "Redirect", url = Url.Action("Index", "News")});
            }
            else
            {
                ModelState.AddModelError("", "Неверный логин и (или) пароль");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            Log.Information("User logged out");
            return RedirectToAction("Index", "News");
        }

        public IActionResult UpdateUser() => View();

        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserCabinetViewModel model)
        {
            if (!ModelState.IsValid) return View("UserCabinet", model);

            var user = await _userManager.FindByIdAsync(model.Id.ToString());

            if (user == null) return View("UserCabinet", model);

            _ = _mapper.Map<UserCabinetViewModel, User>(model, user);

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return View("UserCabinet", model);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View("UserCabinet", model);
        }

        public IActionResult UserInfo()
        {
            return HttpContext.User.Identity is { IsAuthenticated: true } ? View("_Logout") : View("_Login");
        }
    }
}
