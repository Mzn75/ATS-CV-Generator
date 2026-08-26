using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATS_CV_Generator.Models
{
    public class Certificates
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Certificate name is required")]
        [Display(Name = "Certificate Name", Prompt = "e.g., Python Fundanmentals")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Issuing organization is required")]
        [Display(Name = "Issuing Organization", Prompt = "e.g., Huawei")]
        public string? Issuer { get; set; }

        [Required(ErrorMessage = "Issue date is required")]
        [Display(Name = "Date Earned", Prompt = "e.g., August 2026")]
        public string? IssueDate { get; set; }

        [Display(Name = "Credential URL", Prompt = "e.g., https://...")]
        public string? CredentialUrl { get; set; }

        [ForeignKey("CvDraft")]
        public int CvDraftId { get; set; }

        public CvDraft? CvDraft { get; set; }
    }
}
