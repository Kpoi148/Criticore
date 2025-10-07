using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot.Infrastructure.Client.Interfaces
{
    public interface IAiRAGProxy
    {
        Task<string> CallRAGAsync(string prompt, string userId, string userName, string classId, int k = 8, float temperature = 0.7f);
    }
    
}
