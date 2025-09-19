using System;

namespace Class.Domain.DTOs
{
    public class ClassMemberDto
    {
        public int ClassMemberId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string RoleInClass { get; set; } = string.Empty;
        public DateTime? JoinedAt { get; set; }
        public int? GroupId { get; set; }
    }
}