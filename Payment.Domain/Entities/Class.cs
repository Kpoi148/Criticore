using System;
using System.Collections.Generic;

namespace Payment.Domain.Entities;

public partial class Class
{
    public int ClassId { get; set; }

    public string ClassName { get; set; } = null!;

    public string SubjectCode { get; set; } = null!;

    public string? Semester { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? JoinCode { get; set; }

    public virtual ICollection<Aidocument> Aidocuments { get; set; } = new List<Aidocument>();

    public virtual ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();

    public virtual ICollection<ClassMember> ClassMembers { get; set; } = new List<ClassMember>();

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<JoinRequest> JoinRequests { get; set; } = new List<JoinRequest>();

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();

    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
