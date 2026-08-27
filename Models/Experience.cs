using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ATS_CV_Generator.Models
{
    public class Experience
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser? User { get; set; }

        [Required(ErrorMessage = "Please enter your job title.")]
        public string JobTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the company name.")]
        public string Company { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the company location.")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select job starting date.")]
        public string StartDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select job ending date.")]
        public string EndDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a brief description.")]
        public string Description { get; set; } = string.Empty;
        [ForeignKey("CvDraft")]
        public int CvDraftId { get; set; }

        public CvDraft? CvDraft { get; set; }
    }
}
