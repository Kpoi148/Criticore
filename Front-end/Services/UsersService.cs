using Front_end.DTOs;
using Front_end.Services.Interfaces;

namespace Front_end.Services
{
    public class UsersService : IUsersService
    {
        private readonly HttpClient _http;
        public UsersService(HttpClient http) => _http = http;

        public async Task<List<UserDto>> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<UserDto>>("api/user")
                   ?? new List<UserDto>();
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<UserDto>($"api/user/{id}");
        }

        public async Task<bool> BanAsync(int id)
        {
            var res = await _http.PutAsync($"api/user/{id}/ban", null);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> UnbanAsync(int id)
        {
            var res = await _http.PutAsync($"api/user/{id}/unban", null);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> ChangeRoleAsync(int id, int roleType)
        {
            var res = await _http.PutAsync($"api/user/{id}/role?roleType={roleType}", null);
            return res.IsSuccessStatusCode;
        }
        public async Task<List<UserDto>> GetTeachersAsync()
        {
            return await _http.GetFromJsonAsync<List<UserDto>>("api/user/teachers")
                   ?? new List<UserDto>();
        }
    }
}
