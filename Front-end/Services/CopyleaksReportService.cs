using Front_end.Models;
using Front_end.Services.Interfaces;
using System.Net.Http.Json;

namespace Front_end.Services
{
    public class CopyleaksReportService : ICopyleaksReportService
    {
        private readonly HttpClient _http;
        public CopyleaksReportService(HttpClient http)
        {
            _http = http;
            //_http.BaseAddress = new Uri("https://homework.criticore.edu.vn:7154/"); // base URL của Homework API
            _http.BaseAddress = new Uri("https://localhost:7154/"); // base URL của Homework API
        }
        public async Task<List<CopyleaksReportDto>> GetReportsByUserAsync(int userId)
        {
            var response = await _http.GetFromJsonAsync<ApiResponse<List<CopyleaksReportDto>>>($"api/CopyleaksReport/user/{userId}");
            return response?.Data ?? new List<CopyleaksReportDto>();

        }

        public async Task<CopyleaksReportDto?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<CopyleaksReportDto>($"api/reports/{id}");
        }
    }
}
