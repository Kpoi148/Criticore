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
        public async Task<List<ClassSummaryDto>> GetClassesByUserAsync(int userId)
        {
            return await _http.GetFromJsonAsync<List<ClassSummaryDto>>($"api/classes/by-user/{userId}")
                   ?? new List<ClassSummaryDto>();
        }
        public async Task<List<ClassMemberDto>> GetTeachersByClassAsync(int classId)
        {
            return await _http.GetFromJsonAsync<List<ClassMemberDto>>($"api/classes/{classId}/teachers")
                   ?? new List<ClassMemberDto>();
        }
        // phân công giáo viên cho lớp học
        public async Task<bool> AssignTeacherAsync(int classId, int teacherId)
        {
            var res = await _http.PutAsync($"api/classes/{classId}/assign-teacher/{teacherId}", null);
            return res.IsSuccessStatusCode;
        }
        // nhập mã vào lớp học
        public async Task<ClassDto?> JoinByCodeAsync(string code, int userId)
        {
            var res = await _http.PostAsJsonAsync("api/classes/join-by-code", new
            {
                joinCode = code,
                userId = userId
            });

            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<ClassDto>();
        }
        // Đếm số lượng topic của class
        public async Task<int> GetTopicCountByClassAsync(int classId)
        {
            try
            {
                var topics = await _http.GetFromJsonAsync<List<TopicDto>>($"api/Topics/byclass/{classId}");
                return topics?.Count ?? 0;
            }
            catch
            {
                return 0;
            }
        }

    }
}
