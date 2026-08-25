using ATS_CV_Generator.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ATS_CV_Generator.Controllers
{
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("Home/Error404")]
        public IActionResult Error404()
        {
            return View("Error404");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
