using Identity.Domain;
using Identity.Domain.Repositories;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.Domain.Entities;

namespace Identity.Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository) { _userRepository = userRepository; }

    public async Task<User> CreateOrGetUserFromGoogleAsync(ClaimsPrincipal principal)
    {
        var claims = principal.Claims;
        var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(email))
            throw new Exception("Invalid Google claims");

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            user = new User
            {
                FullName = name ?? "Unknown",
                Email = email,
                PasswordHash = string.Empty,  // Để trống cho external auth
                Role = "User",  // Default role
                Status = "Active",
                AvatarUrl = claims.FirstOrDefault(c => c.Type == "picture")?.Value,  // Nếu có từ Google
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
        }
        else
        {
            // Cập nhật thông tin nếu cần
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.SaveChangesAsync();
        }

        return user;
    }
}