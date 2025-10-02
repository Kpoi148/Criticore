using Front_end.DTOs;

namespace Front_end.Services.Interfaces
{
    public interface IUsersService
    {
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<bool> BanAsync(int id);
        Task<bool> UnbanAsync(int id);
        Task<bool> ChangeRoleAsync(int id, int roleType); // 1=User, 2=Teacher
        Task<List<UserDto>> GetTeachersAsync();
    }
}
