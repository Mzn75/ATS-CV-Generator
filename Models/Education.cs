namespace ATS_CV_Generator.Models
{
    public class Education
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser? User { get; set; }

        public string Degree { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
    }
}
