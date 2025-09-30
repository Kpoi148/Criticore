using System;
using System.Collections.Generic;

namespace TopicDetail.Domain.Models;

public partial class ClassMember
{
    public int ClassMemberId { get; set; }

    public int ClassId { get; set; }

    public int UserId { get; set; }

    public string? RoleInClass { get; set; }

    public int? GroupId { get; set; }

    public DateTime? JoinedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual Group? Group { get; set; }

    public virtual User User { get; set; } = null!;
}
