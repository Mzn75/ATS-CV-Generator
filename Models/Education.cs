using System.ComponentModel.DataAnnotations.Schema;

namespace ATS_CV_Generator.Models
{
    public class Education
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser? User { get; set; }

        public string Degree { get; set; } = string.Empty;
        public string Major { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string GradDate { get; set; } = string.Empty;
        [ForeignKey("CvDraft")]
        public int CvDraftId { get; set; }

        public CvDraft? CvDraft { get; set; }
    }
}
