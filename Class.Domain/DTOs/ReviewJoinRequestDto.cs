using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class.Domain.DTOs
{
    // DTO dùng để duyệt yêu cầu tham gia lớp học
    public class ReviewJoinRequestDto
    {
        public int JoinRequestId { get; set; }
        public string Status { get; set; } = null!; // "Approved" hoặc "Rejected"
        public int ReviewedBy { get; set; }
    }

}
