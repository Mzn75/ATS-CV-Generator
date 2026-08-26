using System.ComponentModel.DataAnnotations.Schema;

namespace ATS_CV_Generator.Models
{
    public class ProjectItem
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser? User { get; set; }

        public string ProjectName { get; set; } = string.Empty;
        public string Technologies { get; set; } = string.Empty;
        public string DateRange { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [ForeignKey("CvDraft")]
        public int CvDraftId { get; set; }

        public CvDraft? CvDraft { get; set; }
    }
}
