using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot.Domain.Models
{
    public class AiResponse
    {
        public string Reply { get; set; } = string.Empty;
        public string CognitiveEngagement { get; set; } = string.Empty;
        public List<Message> History { get; set; } = new();
    }
}
