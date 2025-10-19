using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.Domain.DTOs
{
    public class WebhookResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
