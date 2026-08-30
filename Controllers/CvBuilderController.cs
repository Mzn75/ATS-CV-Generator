using ATS_CV_Generator.Data;
using ATS_CV_Generator.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ATS_CV_Generator.Controllers
{
    [Authorize]
    public class CvBuilderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CvBuilderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1.1. Personal Info GET
        [HttpGet]
        public async Task<IActionResult> PersonalInfo()
        {

            var user = await _userManager.GetUserAsync(User);
            var draft = await _context.CvDrafts.FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (draft == null)
            {
                // Pre-fill if no draft exists
                draft = new CvDraft
                {
                    FullName = user.FullName,
                    Email = user.Email
                };
            }

            return View(draft);
        }

        // 1.2. Personal Info POST
        [HttpPost]
        public async Task<IActionResult> PersonalInfo(CvDraft model)
        {
            var user = await _userManager.GetUserAsync(User);
            var existingDraft = await _context.CvDrafts.FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (existingDraft == null)
            {
                model.UserId = user.Id;
                model.LastModified = System.DateTime.Now;
                _context.CvDrafts.Add(model);
            }
            else
            {
                existingDraft.FullName = model.FullName;
                existingDraft.Email = model.Email;
                existingDraft.PhoneNumber = model.PhoneNumber;
                existingDraft.JobTitle = model.JobTitle;
                existingDraft.Country = model.Country;
                existingDraft.City = model.City;
                existingDraft.ProfessionalSummary = model.ProfessionalSummary;
                existingDraft.LastModified = System.DateTime.Now;
            }

            await _context.SaveChangesAsync();

            // Route them to Step 2
            return RedirectToAction("Education");
        }

        // 2.1. Education GET
        [HttpGet]
        public async Task<IActionResult> Education()
        {
            var user = await _userManager.GetUserAsync(User);

            // Grab all user saved degrees to show on the page.
            var draft = await _context.CvDrafts
                .Include(d => d.Educations)
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            // Force them back to step 1 if they try to skip ahead
            if (draft == null) return RedirectToAction("PersonalInfo");

            return View(draft);
        }

        // 2.2. Education POST
        [HttpPost]
        public async Task<IActionResult> AddEducation(CvDraft model)
        {
            var user = await _userManager.GetUserAsync(User);
            var draft = await _context.CvDrafts
                .Include(d => d.Educations)
                .FirstOrDefaultAsync(d => d.Id == model.Id && d.UserId == user.Id);

            if (draft == null) return RedirectToAction("Education");

            // 1. Force the controller to read the button value directly
            string actionType = Request.Form["actionType"];

            bool isFormEmpty = string.IsNullOrWhiteSpace(model.NewEducation?.Degree) &&
                               string.IsNullOrWhiteSpace(model.NewEducation?.Institution);

            // 2. If they clicked Next on an empty form, bypass validation and redirect
            if (actionType == "next" && isFormEmpty)
            {
                return RedirectToAction("Experience");
            }

            // 3. Otherwise, validate the data
            ModelState.Clear();
            TryValidateModel(model.NewEducation);

            if (!ModelState.IsValid)
            {
                return View("Education", draft);
            }

            // 4. Save the data
            if (model.NewEducation != null)
            {
                model.NewEducation.CvDraftId = model.Id;
                model.NewEducation.UserId = user.Id;
                _context.Educations.Add(model.NewEducation);
                await _context.SaveChangesAsync();
            }

            // 5. Final routing check
            if (actionType == "next")
            {
                return RedirectToAction("Experience");
            }

            // Refreshes the page if they clicked "Save & Add Another"
            return RedirectToAction("Education");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEducation(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            // Find the education entry and ensure it belongs to the logged-in user
            var edu = await _context.Educations
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);

            if (edu != null)
            {
                _context.Educations.Remove(edu);
                await _context.SaveChangesAsync();
            }

            // Refresh the page to show the updated list
            return RedirectToAction("Education");
        }

        // 3.1. Experience GET
        [HttpGet]
        public async Task<IActionResult> Experience()
        {
            var user = await _userManager.GetUserAsync(User);

            // Include existing experiences to display in the list
            var draft = await _context.CvDrafts
                .Include(d => d.Experiences)
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (draft == null) return RedirectToAction("PersonalInfo");

            return View(draft);
        }

        // 3.2. Experience POST
        [HttpPost]
        public async Task<IActionResult> AddExperience(CvDraft model)
        {
            var user = await _userManager.GetUserAsync(User);
            var draft = await _context.CvDrafts
                .Include(d => d.Experiences)
                .FirstOrDefaultAsync(d => d.Id == model.Id && d.UserId == user.Id);

            if (draft == null) return RedirectToAction("Experience");

            // 1. Force the controller to read the button value directly
            string actionType = Request.Form["actionType"];

            bool isFormEmpty = string.IsNullOrWhiteSpace(model.NewExperience?.JobTitle) &&
                               string.IsNullOrWhiteSpace(model.NewExperience?.Company);

            // 2. If they clicked Next on an empty form, bypass validation and redirect
            if (actionType == "next" && isFormEmpty)
            {
                return RedirectToAction("Projects");
            }

            // 3. Otherwise, validate the data
            ModelState.Clear();
            TryValidateModel(model.NewExperience);

            if (!ModelState.IsValid)
            {
                return View("Experience", draft);
            }

            // 4. Save the data
            if (model.NewExperience != null)
            {
                model.NewExperience.CvDraftId = model.Id;
                model.NewExperience.UserId = user.Id;
                _context.Experiences.Add(model.NewExperience);
                await _context.SaveChangesAsync();
            }

            // 5. Final routing check
            if (actionType == "next")
            {
                return RedirectToAction("Projects");
            }

            // Refreshes the page if they clicked "Save & Add Another"
            return RedirectToAction("Experience");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteExperience(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            // Find the education entry and ensure it belongs to the logged-in user
            var exp = await _context.Experiences
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);

            if (exp != null)
            {
                _context.Experiences.Remove(exp);
                await _context.SaveChangesAsync();
            }

            // Refresh the page to show the updated list
            return RedirectToAction("Experience");
        }

        // 4.1. Projects GET
        [HttpGet]
        public async Task<IActionResult> Projects()
        {
            var user = await _userManager.GetUserAsync(User);

            var draft = await _context.CvDrafts
                .Include(d => d.Projects)
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (draft == null) return RedirectToAction("PersonalInfo");

            return View(draft);
        }

        // 4.2. Projects POST
        [HttpPost]
        public async Task<IActionResult> AddProject(CvDraft model)
        {
            var user = await _userManager.GetUserAsync(User);
            var draft = await _context.CvDrafts
                .Include(d => d.Projects)
                .FirstOrDefaultAsync(d => d.Id == model.Id && d.UserId == user.Id);

            if (draft == null) return RedirectToAction("Projects");

            string actionType = Request.Form["actionType"];

            bool isFormEmpty = string.IsNullOrWhiteSpace(model.NewProject?.ProjectName);

            if (actionType == "next" && isFormEmpty)
            {
                return RedirectToAction("Certificates");
            }

            ModelState.Clear();
            TryValidateModel(model.NewProject);

            if (!ModelState.IsValid)
            {
                return View("Projects", draft);
            }

            if (model.NewProject != null)
            {
                model.NewProject.CvDraftId = model.Id;
                model.NewProject.UserId = user.Id;
                _context.Projects.Add(model.NewProject);
                await _context.SaveChangesAsync();
            }

            if (actionType == "next")
            {
                return RedirectToAction("Certificates");
            }

            return RedirectToAction("Projects");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            // Find the education entry and ensure it belongs to the logged-in user
            var proj = await _context.Projects
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);

            if (proj != null)
            {
                _context.Projects.Remove(proj);
                await _context.SaveChangesAsync();
            }

            // Refresh the page to show the updated list
            return RedirectToAction("Projects");
        }

        // 5.1. Certificates GET
        [HttpGet]
        public async Task<IActionResult> Certificates()
        {
            var user = await _userManager.GetUserAsync(User);

            var draft = await _context.CvDrafts
                .Include(d => d.Certificates)
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (draft == null) return RedirectToAction("PersonalInfo");

            return View(draft);
        }

        // 5.2. Certificates POST
        [HttpPost]
        public async Task<IActionResult> AddCertificate(CvDraft model)
        {
            var user = await _userManager.GetUserAsync(User);
            var draft = await _context.CvDrafts
                .Include(d => d.Certificates)
                .FirstOrDefaultAsync(d => d.Id == model.Id && d.UserId == user.Id);

            if (draft == null) return RedirectToAction("Certificates");

            string actionType = Request.Form["actionType"];

            bool isFormEmpty = string.IsNullOrWhiteSpace(model.NewCertificate?.Name) &&
                               string.IsNullOrWhiteSpace(model.NewCertificate?.Issuer);

            if (actionType == "next" && isFormEmpty)
            {
                return RedirectToAction("Skills");
            }

            ModelState.Clear();
            TryValidateModel(model.NewCertificate);

            if (!ModelState.IsValid)
            {
                return View("Certificates", draft);
            }

            if (model.NewCertificate != null)
            {
                model.NewCertificate.CvDraftId = model.Id;
                model.NewCertificate.UserId = user.Id;
                _context.Certificates.Add(model.NewCertificate);
                await _context.SaveChangesAsync();
            }

            if (actionType == "next")
            {
                return RedirectToAction("Skills");
            }

            return RedirectToAction("Certificates");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCertificate(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            // Find the education entry and ensure it belongs to the logged-in user
            var cert = await _context.Certificates
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);

            if (cert != null)
            {
                _context.Certificates.Remove(cert);
                await _context.SaveChangesAsync();
            }

            // Refresh the page to show the updated list
            return RedirectToAction("Certificates");
        }

        // 6.1. Skills GET
        [HttpGet]
        public async Task<IActionResult> Skills()
        {
            var user = await _userManager.GetUserAsync(User);

            ViewBag.StandardSkills = await _context.PreDefinedSkills
                .OrderBy(s => s.Name)
                .ToListAsync();

            var draft = await _context.CvDrafts
                .Include(d => d.Skills)
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (draft == null) return RedirectToAction("PersonalInfo");

            return View(draft);
        }

        // 6.2. Skills POST
        [HttpPost]
        public async Task<IActionResult> AddSkill(CvDraft model)
        {
            var user = await _userManager.GetUserAsync(User);
            var draft = await _context.CvDrafts
                .Include(d => d.Skills)
                .FirstOrDefaultAsync(d => d.Id == model.Id && d.UserId == user.Id);

            if (draft == null) return RedirectToAction("Skills");

            string actionType = Request.Form["actionType"];

            bool isFormEmpty = string.IsNullOrWhiteSpace(model.NewSkill?.Name) &&
                string.IsNullOrWhiteSpace(model.NewSkill?.Category);

            if (actionType == "next" && isFormEmpty)
            {
                return RedirectToAction("Result");
            }

            ModelState.Clear();
            TryValidateModel(model.NewSkill);

            if (!ModelState.IsValid)
            {
                ViewBag.StandardSkills = await _context.PreDefinedSkills
                    .OrderBy(s => s.Name)
                    .ToListAsync();

                return View("Skills", draft);
            }

            if (model.NewSkill != null)
            {
                model.NewSkill.CvDraftId = model.Id;
                model.NewSkill.UserId = user.Id;
                _context.Skills.Add(model.NewSkill);
                await _context.SaveChangesAsync();
            }

            if (actionType == "next")
            {
                return RedirectToAction("Result");
            }

            return RedirectToAction("Skills");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            // Find the education entry and ensure it belongs to the logged-in user
            var skl = await _context.Skills
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);

            if (skl != null)
            {
                _context.Skills.Remove(skl);
                await _context.SaveChangesAsync();
            }

            // Refresh the page to show the updated list
            return RedirectToAction("Skills");
        }

        // 7. Result
        [HttpGet]
        public async Task<IActionResult> Result()
        {
            var user = await _userManager.GetUserAsync(User);

            var draft = await _context.CvDrafts
                .Include(d => d.Educations)
                .Include(d => d.Experiences)
                .Include(d => d.Projects)
                .Include(d => d.Certificates)
                .Include(d => d.Skills)
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (draft == null) return RedirectToAction("PersonalInfo");

            return View(draft);
        }
    }
}
