using ATS_CV_Generator.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ATS_CV_Generator.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName // Saving their name right at signup
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    // Redirect directly to the CV editor after signup
                    return RedirectToAction("Index", "Resume");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // PasswordSignInAsync handles checking the hashed password securely
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    isPersistent: model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Resume");
                }

                // If the login fails, return a generic error message for security
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            // Redirect the user to the login page (or homepage) after logging out
            return RedirectToAction("Login", "Account");
        }
    }
}
