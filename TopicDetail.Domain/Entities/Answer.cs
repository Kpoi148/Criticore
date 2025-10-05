using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // Thêm namespace này cho [NotMapped]

namespace TopicDetail.Domain.Models;

public partial class Answer
{
    public int AnswerId { get; set; }
    public int TopicId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual Topic Topic { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();

    [NotMapped] // Không lưu vào DB, tính động
    public double Rating => Votes.Any() ? Votes.Average(v => v.Amount ?? 0) : 0; // Tính average, fallback 0 nếu Amount null

    [NotMapped] // Không lưu vào DB
    public int VoteCount => Votes.Count;
}