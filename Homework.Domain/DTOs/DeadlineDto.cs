using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.Domain.DTOs
{
    public class DeadlineDto
    {
        public int HomeworkId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TopicTitle { get; set; } = string.Empty; // Thêm
        public string ClassName { get; set; } = string.Empty; // Thêm nếu cần
        public DateTime? DueDate { get; set; }
        public string? Status { get; set; }
    }
}
