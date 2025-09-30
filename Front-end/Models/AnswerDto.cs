using System;

namespace Front_end.Models
{
    public class AnswerDto
    {
        public int AnswerId { get; set; }
        public int TopicId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? AvatarUrl { get; set; }
        public string? CreatedBy { get; set; }  // Thêm trường này (từ User.FullName)
    }

    public class CreateAnswerDto
    {
        public int TopicId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = null!;
    }

    public class UpdateAnswerDto
    {
        public string? Content { get; set; }
    }
}