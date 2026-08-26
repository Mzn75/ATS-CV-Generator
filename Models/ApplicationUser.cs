using Microsoft.AspNetCore.Identity;
using ATS_CV_Generator.Models;
using System.ComponentModel.DataAnnotations;

namespace ATS_CV_Generator.Models
{
    public class ApplicationUser : IdentityUser
    {
        //IdentityUser already includes main user data
        [Required(ErrorMessage = "Please enter your full name.")]
        public string? FullName { get; set; }

        // Link data directly to the user
        public virtual ICollection<Experience> Experiences { get; set; } = new List<Experience>();
        public virtual ICollection<Education> Educations { get; set; } = new List<Education>();
        public virtual ICollection<ProjectItem> Projects { get; set; } = new List<ProjectItem>();
        public virtual ICollection<Certificates> Certificates { get; set; } = new List<Certificates>();
    }
}
