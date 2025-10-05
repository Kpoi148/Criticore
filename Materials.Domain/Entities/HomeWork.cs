//using System;
//using System.Collections.Generic;

//namespace Material.Domain.Entities;

//public partial class HomeWork
//{
//    public int HomeworkId { get; set; }

//    public int TopicId { get; set; }

//    public string Title { get; set; } = null!;

//    public string? Description { get; set; }

//    public string? Status { get; set; }

//    public DateTime? DueDate { get; set; }

//    public int CreatedBy { get; set; }

//    public DateTime? CreatedAt { get; set; }

//    public DateTime? UpdatedAt { get; set; }

//    public virtual User CreatedByNavigation { get; set; } = null!;

//    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();

//    public virtual Topic Topic { get; set; } = null!;
//}
