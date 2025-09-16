namespace Front_end.Models
{
    public class ClassDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public string? Semester { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public int CreatedBy { get; set; }

    }
}
