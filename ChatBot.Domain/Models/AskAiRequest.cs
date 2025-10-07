using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot.Domain.Models
{
    public class AskAiRequest
    {
        public string UserInput { get; set; } = string.Empty;
        public List<Message> History { get; set; } = new();
        public bool UseRAG { get; set; } = false;  // Flag để chọn OpenAI pure hoặc RAG Python
    }
}
