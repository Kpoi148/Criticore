using System;
using System.Collections.Generic;

namespace TopicDetail.Domain.Models;

public partial class Group
{
    public int GroupId { get; set; }

    public int ClassId { get; set; }

    public string GroupName { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual ICollection<ClassMember> ClassMembers { get; set; } = new List<ClassMember>();

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
