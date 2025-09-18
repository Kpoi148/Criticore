using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class.Domain.DTOs
{
    // DTO dùng để gửi yêu cầu tham gia lớp học
    public class JoinRequestDto
    {
        public int JoinRequestId { get; set; }
        public int ClassId { get; set; }
        public int UserId { get; set; }
        public string? Message { get; set; }
        public string Status { get; set; } = "Pending";

        public string? FullName { get; set; }
        public string Email { get; set; } = null!;
    }

}
