using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class.Domain.DTOs
{
    // DTO dùng để tham gia lớp học bằng mã mời
    public class JoinByCodeDto
    {
        public string JoinCode { get; set; } = null!;
        public int UserId { get; set; }
    }

}
