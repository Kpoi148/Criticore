using ChatBot.Application.Services.Interfaces;
using ChatBot.Domain.Models;
using ChatBot.Infrastructure.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot.Application.Services
{
    public class AiInteractionService : IAiInteractionService
    {
        private readonly IOpenAiClient _openAiClient;
        private readonly IAiRAGProxy _ragProxy;
        private readonly string _systemPrompt = @"You are an AI assistant... (paste full Socratic prompt từ code)";

        private readonly string _classifyPrompt = @"Bạn là một bộ phân loại... (paste full classify prompt từ code)";

        public AiInteractionService(IOpenAiClient openAiClient, IAiRAGProxy ragProxy)
        {
            _openAiClient = openAiClient;
            _ragProxy = ragProxy;
        }

        public async Task<AiResponse> AskAiAsync(AskAiRequest request, string userId, string userName, string classId)
        {
            var history = request.History ?? new List<Message>();
            history.Add(new Message { Role = "user", Content = request.UserInput });

            string aiReply;
            if (request.UseRAG)
            {
                aiReply = await _ragProxy.CallRAGAsync(request.UserInput, userId, userName, classId);
            }
            else
            {
                var messages = new List<Message> { new() { Role = "system", Content = _systemPrompt } };
                messages.AddRange(history);
                aiReply = await _openAiClient.GetCompletionAsync(messages);
            }

            history.Add(new Message { Role = "assistant", Content = aiReply });

            string engagement = await ClassifyCognitiveEngagementAsync(request.UserInput);

            return new AiResponse { Reply = aiReply, CognitiveEngagement = engagement, History = history };
        }

        private async Task<string> ClassifyCognitiveEngagementAsync(string userInput)
        {
            var messages = new List<Message>
        {
            new() { Role = "system", Content = _classifyPrompt },
            new() { Role = "user", Content = userInput }
        };

            return await _openAiClient.GetCompletionAsync(messages, temperature: 0.5f, maxTokens: 10);
        }
    }
}
