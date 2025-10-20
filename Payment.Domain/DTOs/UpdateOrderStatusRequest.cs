using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.DTOs
{
    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; } = "Pending";
        public int AdminID { get; set; }
    }
}
