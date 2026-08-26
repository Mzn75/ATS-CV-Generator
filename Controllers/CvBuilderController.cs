using Microsoft.AspNetCore.Mvc;

namespace ATS_CV_Generator.Controllers
{
    public class CvBuilderController : Controller
    {
        public IActionResult PersonalInfo()
        {
            return View();
        }
    }
}
