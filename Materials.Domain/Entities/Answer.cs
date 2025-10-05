//using System;
//using System.Collections.Generic;

//namespace Material.Domain.Entities;

//public partial class Answer
//{
//    public int AnswerId { get; set; }

//    public int TopicId { get; set; }

//    public int UserId { get; set; }

//    public string Content { get; set; } = null!;

//    public DateTime? CreatedAt { get; set; }

//    public DateTime? UpdatedAt { get; set; }

//    public virtual Topic Topic { get; set; } = null!;

//    public virtual User User { get; set; } = null!;

//    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
//}
