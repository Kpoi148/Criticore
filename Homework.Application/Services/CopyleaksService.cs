using Copyleaks.SDK.V3.API;
using Copyleaks.SDK.V3.API.Models.Requests;
using Copyleaks.SDK.V3.API.Models.Requests.Properties;
using Microsoft.AspNetCore.Http;
using Homework.Domain.DTOs;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Homework.Application.Services
{
    public class CopyleaksService
    {
        private readonly string _email = "Haotaca181510@fpt.edu.vn";
        private readonly string _apiKey = "36a282c2-2666-49a7-b528-7898ac5743cd";
        private readonly string _webhookUrl = "https://your-domain.com/api/copyleakswebhook/webhook/{STATUS}";

        public async Task<string> SubmitFileForScanAsync(IFormFile file)
        {
            var identityClient = new CopyleaksIdentityApi();
            var loginResponse = await identityClient.LoginAsync(_email, _apiKey);

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var base64File = Convert.ToBase64String(ms.ToArray());

            var scanProperties = new ClientScanProperties
            {
                Sandbox = false, // false = quét thật (tính phí)
                Webhooks = new Webhooks
                {
                    Status = new Uri(_webhookUrl)
                }
            };

            var fileDocument = new FileDocument
            {
                Base64 = base64File,
                Filename = file.FileName,
                PropertiesSection = scanProperties
            };

            var scanId = Guid.NewGuid().ToString();
            var apiClient = new CopyleaksScansApi();

            await apiClient.SubmitFileAsync(scanId, fileDocument, loginResponse.Token);

            return scanId; // để tra kết quả sau này
        }
    }
}
