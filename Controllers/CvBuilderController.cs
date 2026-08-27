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
                // CREATE: Save their very first draft
                model.UserId = user.Id;
                model.LastModified = System.DateTime.Now;
                _context.CvDrafts.Add(model);
            }
            else
            {
                // UPDATE: Overwrite the existing draft with new form data
                existingDraft.FullName = model.FullName;
                existingDraft.Email = model.Email;
                existingDraft.PhoneNumber = model.PhoneNumber;
                existingDraft.JobTitle = model.JobTitle;
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
    }
}
