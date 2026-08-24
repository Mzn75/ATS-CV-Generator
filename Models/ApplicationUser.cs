using Microsoft.AspNetCore.Identity;
using ATS_CV_Generator.Models;

namespace ATS_CV_Generator.Models
{
    public class ApplicationUser : IdentityUser
    {
        //IdentityUser already includes main user data
        public string? FullName { get; set; }
        public string? GitHubUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? ProfessionalTitle { get; set; }

        // Link data directly to the user
        public virtual ICollection<Experience> Experiences { get; set; } = new List<Experience>();
        public virtual ICollection<Education> Educations { get; set; } = new List<Education>();
        public virtual ICollection<ProjectItem> Projects { get; set; } = new List<ProjectItem>();
    }
}
