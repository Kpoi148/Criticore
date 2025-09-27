using System;

namespace Class.Domain.DTOs
{
    public class ClassMemberDto
    {
        public int ClassMemberId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleInClass { get; set; } = string.Empty;
        public DateTime? JoinedAt { get; set; }
        public int? GroupId { get; set; }
    }
}