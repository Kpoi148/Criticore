using System;

namespace TopicDetail.Application.DTOs
{
    public class AnswerDto
    {
        public int AnswerId { get; set; }
        public int TopicId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
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