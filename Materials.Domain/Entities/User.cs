using System;
using System.Collections.Generic;

namespace Material.Domain.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? Status { get; set; }

    public string? AvatarUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Aidocument> Aidocuments { get; set; } = new List<Aidocument>();

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();

    public virtual ICollection<ClassMember> ClassMembers { get; set; } = new List<ClassMember>();

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<HomeWork> HomeWorks { get; set; } = new List<HomeWork>();

    public virtual ICollection<JoinRequest> JoinRequestReviewedByNavigations { get; set; } = new List<JoinRequest>();

    public virtual ICollection<JoinRequest> JoinRequestUsers { get; set; } = new List<JoinRequest>();

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();

    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();

    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
