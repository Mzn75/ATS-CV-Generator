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

        // GET: Show the Views
        [HttpGet]
        public IActionResult Register() => View();

        [HttpGet]
        public IActionResult Login() => View();


        // POST: Process Registration
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName
                };

                // Hashing Passwords
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Log the user in after they register sending them to the dashboard
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                // If creation fails (e.g. password too weak), send errors to the UI
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }


        // POST: Process Log In
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Securely check the hashed password in the DB
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Dashboard", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }


        // POST: Process Log Out
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            // Send user back to the login page after logging out
            return RedirectToAction("Login", "Account");
        }
    }
}
