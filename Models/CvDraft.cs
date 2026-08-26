using ATS_CV_Generator.Models;
using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATS_CV_Generator.Models
{
    public class CvDraft
    {
        [Key]
        public int Id { get; set; }

        // Links this CV to the specific logged-in user
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public DateTime LastModified { get; set; } = DateTime.Now;

        // 1. Personal Info
        public string? FullName { get; set; }
        public string? Email { get; set; }

        public string? JobTitle { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? GitHubUrl { get; set; }

        public string? ProfessionalSummary { get; set; }

        // 2. Educations
        public List<Education> Educations { get; set; } = new List<Education>();

        [NotMapped]
        public Education NewEducation { get; set; } = new Education();

        // 3. Experiences
        public List<Experience> Experiences { get; set; } = new List<Experience>();

        [NotMapped]
        public Experience NewExperience { get; set; }

        // 4. Projects
        public List<ProjectItem> Projects { get; set; } = new List<ProjectItem>();

        // 6. Certificates
        public List<Certificates> Certificates { get; set; } = new List<Certificates>();
    }
}
