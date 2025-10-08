using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.Domain.DTOs
{
    public class PlagiarismResultDto
    {
        public double PlagiarismScore { get; set; }
        public double AiContentScore { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ReportUrl { get; set; } = string.Empty;
    }
}
