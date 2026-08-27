using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ATS_CV_Generator.Models
{
    public class Education
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser? User { get; set; }

        [Required(ErrorMessage = "Please enter your degree.")]
        public string Degree { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your major.")]
        public string Major { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the institution.")]
        public string Institution { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select your graduation date.")]
        public string GradDate { get; set; } = string.Empty;

        [ForeignKey("CvDraft")]
        public int CvDraftId { get; set; }

        public CvDraft? CvDraft { get; set; }
    }
}
