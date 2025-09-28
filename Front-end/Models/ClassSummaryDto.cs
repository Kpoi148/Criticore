using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Front_end.Models
{
    public class ClassSummaryDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public string JoinCode { get; set; } = string.Empty;
        public int MemberCount { get; set; }      // số học viên
        public string Teacher { get; set; } = ""; // tên giáo viên
    }
}
