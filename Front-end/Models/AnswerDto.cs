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
        public string? AvatarUrl { get; set; }
        public string? CreatedBy { get; set; } // Thêm trường này (từ User.FullName)
        public double Rating { get; set; } // Average rating từ Votes (thêm trường này)
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
    // Thêm DTO cho Vote
    public class VoteDto
    {
        public int VoteId { get; set; }
        public int AnswerId { get; set; }
        public int UserId { get; set; }
        public string VoteType { get; set; } = null!;
        public int Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class CreateVoteDto
    {
        public int AnswerId { get; set; }
        public int UserId { get; set; }
        public string VoteType { get; set; } = null!;
        public int Amount { get; set; }
    }
    public class UpdateVoteDto
    {
        public string? VoteType { get; set; }
        public int? Amount { get; set; }
    }
}