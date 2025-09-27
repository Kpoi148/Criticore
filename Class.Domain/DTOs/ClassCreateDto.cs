using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class.Domain.DTOs
{
    public class ClassCreateDto
    {
        public string ClassName { get; set; } = null!;
        public string SubjectCode { get; set; } = null!;
        public string? Semester { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public int CreatedBy { get; set; }
    }
}
