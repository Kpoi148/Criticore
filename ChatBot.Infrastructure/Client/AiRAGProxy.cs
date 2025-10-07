using ChatBot.Infrastructure.Client.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot.Infrastructure.Client
{
    public class AiRAGProxy : IAiRAGProxy
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AiRAGProxy(IHttpClientFactory factory, IConfiguration config)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri(config["AiBackendUrl"] ?? "http://localhost:8000/");
            _config = config;
        }

        public async Task<string> CallRAGAsync(string prompt, string userId, string userName, string classId, int k, float temperature)
        {
            var request = new
            {
                prompt,
                user_profile = new
                {
                    user_id = userId,
                    preferences = new { },
                    context = $"User: {userName}, Class: {classId}"
                },
                k,
                temperature
            };

            var response = await _httpClient.PostAsJsonAsync("chat", request);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<dynamic>();
            return data?.response?.ToString() ?? string.Empty;
        }
    }
}
