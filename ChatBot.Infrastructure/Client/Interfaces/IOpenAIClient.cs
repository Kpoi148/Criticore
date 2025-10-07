using ChatBot.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot.Infrastructure.Client.Interfaces
{
    public interface IOpenAiClient
    {
        Task<string> GetCompletionAsync(List<Message> messages, string model = "gpt-3.5-turbo", float temperature = 0.7f, int maxTokens = 512);
    }    
}
