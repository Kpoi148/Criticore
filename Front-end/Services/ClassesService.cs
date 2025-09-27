using Front_end.Models;
using Front_end.Services.Interfaces;

namespace Front_end.Services
{
    public class ClassesService : IClassesService
    {
        private readonly HttpClient _http;
        public ClassesService(HttpClient http) => _http = http;

        public async Task<List<ClassDto>> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<ClassDto>>("api/classes")
                   ?? new List<ClassDto>();
        }

        public async Task<ClassDto?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<ClassDto>($"api/classes/{id}");
        }

        public async Task<int?> CreateAsync(ClassCreateDto dto)
        {
            var res = await _http.PostAsJsonAsync("api/classes", dto);
            if (!res.IsSuccessStatusCode) return null;

            var created = await res.Content.ReadFromJsonAsync<ClassDto>();
            return created?.ClassId;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var res = await _http.DeleteAsync($"api/classes/{id}");
            return res.IsSuccessStatusCode;
        }
        public async Task<bool> UpdateAsync(int id, ClassDto dto)
        {
            var res = await _http.PutAsJsonAsync($"api/classes/{id}", dto);
            return res.IsSuccessStatusCode;
        }

    }
}
