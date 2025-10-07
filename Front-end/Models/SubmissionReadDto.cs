using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Front_end.Models
{
    public class SubmissionReadDto
    {
        public int SubmissionId { get; set; }
        public int HomeworkId { get; set; }
        public int UserId { get; set; }
        public int? GroupId { get; set; }
        public string? Content { get; set; }
        public string? AttachmentUrl { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Optional: thêm thông tin liên quan nếu muốn hiển thị chi tiết hơn
        public string? UserName { get; set; }
        public string? GroupName { get; set; }
    }
}
