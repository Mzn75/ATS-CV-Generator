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
        public async Task<IActionResult> AddEducation(int cvId, CvDraft model)
        {
            var user = await _userManager.GetUserAsync(User);

            // Double-check the draft actually belongs to the logged-in user for security
            var draft = await _context.CvDrafts.FirstOrDefaultAsync(d => d.Id == cvId && d.UserId == user.Id);

            if (model.NewEducation != null)
            {
                // Link the new degree to this specific CV Draft
                model.NewEducation.CvDraftId = cvId;

                // Save only the new degree to the database
                _context.Educations.Add(model.NewEducation);
                await _context.SaveChangesAsync();
            }

            // Refresh the page so the user sees the newly added degree in the list above
            return RedirectToAction("Education");
        }

    }
}
