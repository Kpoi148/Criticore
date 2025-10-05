//using System;
//using System.Collections.Generic;

//namespace Material.Domain.Entities;

//public partial class Submission
//{
//    public int SubmissionId { get; set; }

//    public int HomeworkId { get; set; }

//    public int UserId { get; set; }

//    public int? GroupId { get; set; }

//    public string? Content { get; set; }

//    public string? AttachmentUrl { get; set; }

//    public DateTime? SubmittedAt { get; set; }

//    public DateTime? UpdatedAt { get; set; }

//    public virtual Group? Group { get; set; }

//    public virtual HomeWork Homework { get; set; } = null!;

//    public virtual User User { get; set; } = null!;
//}
