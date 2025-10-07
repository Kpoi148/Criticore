using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Front_end.Models
{
    public class SubmissionCreateDto
    {
        public int HomeworkId { get; set; }
        public int UserId { get; set; }
        public int? GroupId { get; set; }
        public string? Content { get; set; }
        public string? AttachmentUrl { get; set; }
    }
}
