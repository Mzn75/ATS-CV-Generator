using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATS_CV_Generator.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Category { get; set; }

        [ForeignKey("CvDraft")]
        public int CvDraftId { get; set; }
        [ValidateNever]
        public CvDraft? CvDraft { get; set; }
    }
}
