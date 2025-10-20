using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Front_end.Models
{
    public class CreateOrderRequest
    {
        public int UserId { get; set; }                // ID của người dùng (lấy từ User Service)
        public string PackageName { get; set; } = "Gói 1 tháng";
        public decimal Price { get; set; } = 100000m;
        public int DurationInMonths { get; set; } = 1;
        public string Status { get; set; } = "Pending";
        public DateTime? StartDate { get; set; }       // Backend có thể auto set
        public DateTime? EndDate { get; set; }         // Có thể để null, backend tính
        public int? ConfirmedBy { get; set; }          // ID admin xác nhận, để null khi tạo    }
    }
}
