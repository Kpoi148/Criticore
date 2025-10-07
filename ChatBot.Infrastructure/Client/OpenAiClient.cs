using ChatBot.Domain.Models;
using ChatBot.Infrastructure.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace ChatBot.Infrastructure.Client
{
    public class OpenAiClient : IOpenAiClient
    {
        private readonly HttpClient _httpClient;

        public OpenAiClient(HttpClient httpClient) // Chỉ nhận HttpClient
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<string> GetCompletionAsync(List<Message> messages, string model, float temperature, int maxTokens)
        {
            var request = new
            {
                model,
                messages,
                temperature,
                max_tokens = maxTokens,
                top_p = 1.0,
                stream = false,
                stop = (string?)null
            };

            var response = await _httpClient.PostAsJsonAsync("chat/completions", request);

            // Đọc lỗi rõ ràng hơn nếu có vấn đề
            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                throw new Exception($"Lỗi HTTP {response.StatusCode}: {errorText}");
            }

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("choices", out var choices))
            {
                var content = choices[0].GetProperty("message").GetProperty("content").GetString();
                return content?.Trim() ?? string.Empty;
            }

            return "⚠️ Không nhận được phản hồi hợp lệ từ OpenAI.";
        }
    }
}