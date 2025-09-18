using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class.Domain.DTOs
{
    // DTO dùng để gửi yêu cầu tham gia lớp học
    public class CreateJoinRequestDto
    {
        public int ClassId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
    }
}
