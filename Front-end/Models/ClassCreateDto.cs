﻿namespace Front_end.Models
{
    public class ClassCreateDto
    {
        public string ClassName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public string? Semester { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public int CreatedBy { get; set; }
        public int TeacherId { get; set; }
        public string? JoinCode { get; set; }
    }
}
