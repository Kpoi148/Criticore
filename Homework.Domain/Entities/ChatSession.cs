using System;
using System.Collections.Generic;

namespace Homework.Domain.Entities;

public partial class ChatSession
{
    public int ChatSessionId { get; set; }

    public int ClassId { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
