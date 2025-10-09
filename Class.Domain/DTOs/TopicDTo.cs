using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class.Domain.DTOs
{
    public class TopicDto
    {
        public int TopicId { get; set; }
        public int ClassId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Type { get; set; }
        public DateTime? EndTime { get; set; }
        public int CreatedBy { get; set; }
        public string? CreatedByName { get; set; }  // Thêm trường này để lưu tên người tạo
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateTopicDto
    {
        public int ClassId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Type { get; set; }
        public DateTime? EndTime { get; set; }
        public int CreatedBy { get; set; }
    }

    public class UpdateTopicDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Type { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
