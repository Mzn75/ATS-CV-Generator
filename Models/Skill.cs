using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATS_CV_Generator.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        [ForeignKey("CvDraft")]
        public int CvDraftId { get; set; }
        public CvDraft CvDraft { get; set; }
    }
}
