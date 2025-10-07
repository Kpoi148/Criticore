using ChatBot.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot.Application.Services.Interfaces
{
    public interface IAiInteractionService
    {
        Task<AiResponse> AskAiAsync(AskAiRequest request, string userId, string userName, string classId);
    }

}
