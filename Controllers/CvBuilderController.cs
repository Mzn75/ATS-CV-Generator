using ATS_CV_Generator.Data;
using ATS_CV_Generator.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
                existingDraft.GitHubUrl = model.GitHubUrl;
                existingDraft.LinkedInUrl = model.LinkedInUrl;
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
            TryValidateModel(model.NewEducation, nameof(model.NewEducation));

            System.Diagnostics.Debug.WriteLine($"GradDate value: '{model.NewEducation?.GradDate}'");
            System.Diagnostics.Debug.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
            foreach (var err in ModelState.Values.SelectMany(v => v.Errors))
            {
                System.Diagnostics.Debug.WriteLine($"Validation error: {err.ErrorMessage}");
            }

            if (!ModelState.IsValid)
            {
                draft.NewEducation = model.NewEducation;
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
            TryValidateModel(model.NewExperience, nameof(model.NewExperience));

            if (!ModelState.IsValid)
            {
                draft.NewExperience = model.NewExperience;
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
            TryValidateModel(model.NewProject, nameof(model.NewProject));

            if (!ModelState.IsValid)
            {
                draft.NewProject = model.NewProject;
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
            TryValidateModel(model.NewCertificate, nameof(model.NewCertificate));

            if (!ModelState.IsValid)
            {
                draft.NewCertificate = model.NewCertificate;
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
            TryValidateModel(model.NewSkill, nameof(model.NewSkill));

            if (!ModelState.IsValid)
            {
                ViewBag.StandardSkills = await _context.PreDefinedSkills
                    .OrderBy(s => s.Name)
                    .ToListAsync();

                draft.NewSkill = model.NewSkill;
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

        private static void RegisterIfExists(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                Console.WriteLine($"[FONT MISSING] {path}"); // <-- temp debug line
                return;
            }
            using var stream = System.IO.File.OpenRead(path);
            QuestPDF.Drawing.FontManager.RegisterFont(stream);
            Console.WriteLine($"[FONT LOADED] {path}");
        }

        public static void RegisterFonts(string webRootPath)
        {
            RegisterIfExists($"{webRootPath}/fonts/SourceSerif4-Regular.ttf");
            RegisterIfExists($"{webRootPath}/fonts/SourceSerif4-Bold.ttf");
            RegisterIfExists($"{webRootPath}/fonts/SourceSerif4-Italic.ttf");
            RegisterIfExists($"{webRootPath}/fonts/SourceSerif4-BoldItalic.ttf");
            RegisterIfExists($"{webRootPath}/fonts/fa-solid-900.ttf");
            RegisterIfExists($"{webRootPath}/fonts/fa-brands-400.ttf");
        }

        // 8. Export PDF Function
        public async Task<IActionResult> ExportPdf()
        {
            var userId = _userManager.GetUserId(User);

            var model = await _context.CvDrafts
                .Include(c => c.Educations)
                .Include(c => c.Experiences)
                .Include(c => c.Projects)
                .Include(c => c.Certificates)
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (model == null)
                return NotFound("No CV found for this user.");

            const string BodyFont = "Source Serif 4";
            const string IconFontSolid = "Font Awesome 7 Free Solid";
            const string IconFontBrands = "Font Awesome 7 Brands";

            const string TextColor = "#1a1a1a";
            const string SubColor = "#333333";
            const string LinkColor = "#0000FF";
            const string SepColor = "#999999";

            byte[] pdfBytes = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginTop(35);
                    page.MarginBottom(25);
                    page.MarginHorizontal(40);

                    page.DefaultTextStyle(x => x
                        .FontFamily(BodyFont)
                        .FontColor(TextColor)
                        .LineHeight(1.4f));

                    page.Content().Column(column =>
                    {
                        column.Spacing(2);

                        // ============ HEADER ============
                        column.Item().AlignCenter()
                            .Text(model.FullName).FontSize(24).Bold();

                        column.Item().PaddingTop(4).AlignCenter()
                            .Text(model.JobTitle).FontSize(12).Bold();

                        column.Item().PaddingTop(8).AlignCenter().Row(row =>
                        {
                            var isFirst = true;
                            void Sep()
                            {
                                if (isFirst) { isFirst = false; return; }
                                row.AutoItem().PaddingHorizontal(6)
                                    .Text("|").FontSize(9).FontColor(SepColor);
                            }

                            Sep();
                            row.AutoItem().Text($"{model.Country}, {model.City}")
                                .FontSize(9).FontColor(SubColor);

                            if (!string.IsNullOrWhiteSpace(model.Email))
                            {
                                Sep();
                                row.AutoItem().Text(t =>
                                {
                                    // \uf0e0 = fa-envelope
                                    t.Span("\uf0e0  ").FontFamily(IconFontSolid).FontSize(9).FontColor(LinkColor);
                                    t.Hyperlink(model.Email, $"mailto:{model.Email}")
                                        .FontSize(9).FontColor(LinkColor);
                                });
                            }

                            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
                            {
                                Sep();
                                row.AutoItem().Text(t =>
                                {
                                    // \uf095 = fa-phone
                                    t.Span("\uf095  ").FontFamily(IconFontSolid).FontSize(9).FontColor(LinkColor);
                                    t.Hyperlink(model.PhoneNumber, $"tel:{model.PhoneNumber}")
                                        .FontSize(9).FontColor(LinkColor);
                                });
                            }

                            if (!string.IsNullOrWhiteSpace(model.GitHubUrl))
                            {
                                Sep();
                                row.AutoItem().Text(t =>
                                {
                                    // \uf09b = fa-github
                                    t.Span("\uf09b  ").FontFamily(IconFontBrands).FontSize(9).FontColor(LinkColor);
                                    t.Hyperlink("GitHub", model.GitHubUrl).FontSize(9).FontColor(LinkColor);
                                });
                            }

                            if (!string.IsNullOrWhiteSpace(model.LinkedInUrl))
                            {
                                Sep();
                                row.AutoItem().Text(t =>
                                {
                                    // \uf08c = fa-linkedin
                                    t.Span("\uf08c  ").FontFamily(IconFontBrands).FontSize(9).FontColor(LinkColor);
                                    t.Hyperlink("LinkedIn", model.LinkedInUrl).FontSize(9).FontColor(LinkColor);
                                });
                            }
                        });

                        // ============ SUMMARY ============
                        if (!string.IsNullOrWhiteSpace(model.ProfessionalSummary))
                        {
                            SectionTitle(column, "Summary", TextColor);
                            column.Item().PaddingTop(4)
                                .Text(model.ProfessionalSummary).FontSize(10);
                        }

                        // ============ EDUCATION ============
                        if (model.Educations != null && model.Educations.Any())
                        {
                            SectionTitle(column, "Education", TextColor);
                            column.Item().PaddingTop(6).Column(col =>
                            {
                                col.Spacing(6);
                                foreach (var edu in model.Educations)
                                {
                                    col.Item().Row(row =>
                                    {
                                        row.RelativeItem().Column(c =>
                                        {
                                            c.Item().Text(edu.Institution).FontSize(11).Bold();
                                            c.Item().Text($"Major: {edu.Major}")
                                                .FontSize(10).Italic().FontColor(SubColor);
                                        });
                                        row.ConstantItem(100).AlignRight().AlignMiddle()
                                            .Text(DateTime.Parse(edu.GradDate).ToString("MMM yyyy"))
                                            .FontSize(9.5f).FontColor(SubColor);
                                    });
                                }
                            });
                        }

                        // ============ EXPERIENCE ============
                        if (model.Experiences != null && model.Experiences.Any())
                        {
                            SectionTitle(column, "Experience", TextColor);
                            column.Item().PaddingTop(6).Column(col =>
                            {
                                col.Spacing(8);
                                foreach (var exp in model.Experiences)
                                {
                                    col.Item().Column(item =>
                                    {
                                        item.Item().Row(row =>
                                        {
                                            row.RelativeItem().Column(c =>
                                            {
                                                c.Item().Text(exp.JobTitle).FontSize(11).Bold();
                                                c.Item().Text(exp.Company)
                                                    .FontSize(10).Italic().FontColor(SubColor);
                                            });
                                            row.ConstantItem(140).AlignRight().AlignMiddle()
                                                .Text($"{DateTime.Parse(exp.StartDate):MMM yyyy} \u2013 {DateTime.Parse(exp.EndDate):MMM yyyy}")
                                                .FontSize(9.5f).FontColor(SubColor);
                                        });

                                        if (!string.IsNullOrWhiteSpace(exp.Description))
                                        {
                                            item.Item().PaddingTop(3).PaddingLeft(14).Text(t =>
                                            {
                                                t.Span("\u2022  ").FontSize(10);
                                                t.Span(exp.Description).FontSize(10);
                                            });
                                        }
                                    });
                                }
                            });
                        }

                        // ============ PROJECTS ============
                        if (model.Projects != null && model.Projects.Any())
                        {
                            SectionTitle(column, "Projects", TextColor);
                            column.Item().PaddingTop(6).Column(col =>
                            {
                                col.Spacing(8);
                                foreach (var proj in model.Projects)
                                {
                                    col.Item().Column(item =>
                                    {
                                        item.Item().Row(row =>
                                        {
                                            row.RelativeItem()
                                                .Text(proj.ProjectName).FontSize(11).Bold();
                                            row.ConstantItem(140).AlignRight().AlignMiddle()
                                                .Text($"{DateTime.Parse(proj.StartDate):MMM yyyy} \u2013 {DateTime.Parse(proj.EndDate):MMM yyyy}")
                                                .FontSize(9.5f).FontColor(SubColor);
                                        });

                                        if (!string.IsNullOrWhiteSpace(proj.Description))
                                        {
                                            item.Item().PaddingTop(3).PaddingLeft(14).Text(t =>
                                            {
                                                t.Span("\u2022  ").FontSize(10);
                                                t.Span(proj.Description).FontSize(10);
                                            });
                                        }
                                    });
                                }
                            });
                        }

                        // ============ CERTIFICATIONS ============
                        if (model.Certificates != null && model.Certificates.Any())
                        {
                            SectionTitle(column, "Certifications", TextColor);
                            column.Item().PaddingTop(6).Column(col =>
                            {
                                col.Spacing(5);
                                foreach (var cert in model.Certificates)
                                {
                                    col.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text(t =>
                                        {
                                            t.Span(cert.Name).FontSize(11).Bold();
                                            t.Span($" \u2013 {cert.Issuer}").FontSize(11);
                                        });
                                        row.ConstantItem(100).AlignRight()
                                            .Text(DateTime.Parse(cert.IssueDate).ToString("MMM yyyy"))
                                            .FontSize(9.5f).FontColor(SubColor);
                                    });
                                }
                            });
                        }

                        // ============ SKILLS ============
                        if (model.Skills != null && model.Skills.Any())
                        {
                            SectionTitle(column, "Skills", TextColor);
                            var grouped = model.Skills
                                .GroupBy(s => string.IsNullOrWhiteSpace(s.Category) ? "Other" : s.Category);

                            column.Item().PaddingTop(6).Column(col =>
                            {
                                col.Spacing(4);
                                foreach (var group in grouped)
                                {
                                    col.Item().Row(row =>
                                    {
                                        row.AutoItem().Text("\u2022  ").FontSize(10.5f);
                                        row.RelativeItem().Text(t =>
                                        {
                                            t.Span($"{group.Key}: ").FontSize(10.5f).Bold();
                                            t.Span(string.Join(", ", group.Select(s => s.Name))).FontSize(10.5f);
                                        });
                                    });
                                }
                            });
                        }
                    });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", $"{model.FullName}_CV.pdf");
        }

        private static void SectionTitle(QuestPDF.Fluent.ColumnDescriptor column, string title, string color)
        {
            column.Item().PaddingTop(14)
                .BorderBottom(1).BorderColor(color)
                .PaddingBottom(4)
                .Text(title).FontSize(12).Bold();
        }

        // 8. Export View "Not Available for users"
        [HttpGet]
        public async Task<IActionResult> ExportView()
        {
            var user = await _userManager.GetUserAsync(User);

            var draft = await _context.CvDrafts
                .Include(d => d.Educations)
                .Include(d => d.Experiences)
                .Include(d => d.Projects)
                .Include(d => d.Certificates)
                .Include(d => d.Skills)
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            return View(draft);
        }


        // 9. New CV - Clears the current draft and starts a new one
        [HttpGet]
        public async Task<IActionResult> NewCv()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var existingDraft = await _context.CvDrafts
                .Include(d => d.Educations)
                .Include(d => d.Experiences)
                .Include(d => d.Projects)
                .Include(d => d.Certificates)
                .Include(d => d.Skills)
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (existingDraft != null)
            {
                _context.Educations.RemoveRange(existingDraft.Educations);
                _context.Experiences.RemoveRange(existingDraft.Experiences);
                _context.Projects.RemoveRange(existingDraft.Projects);
                _context.Certificates.RemoveRange(existingDraft.Certificates);
                _context.Skills.RemoveRange(existingDraft.Skills);

                _context.CvDrafts.Remove(existingDraft);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("PersonalInfo");
        }

    }
}
