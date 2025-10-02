using Identity.Domain;
using Identity.Domain.Repositories;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.Domain.Entities;
using AutoMapper;
using Identity.Domain.DTOs;

namespace Identity.Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
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
                AvatarUrl = claims.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value,  // Nếu có từ Google
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
    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return _mapper.Map<UserDto?>(user);
    }
    //public async Task<List<UserDto>> GetTeachersAsync()
    //{
    //    var users = await _userRepository.GetAllAsync();
    //    return users
    //        .Where(u => u.Role == "Teacher")
    //        .Select(u => _mapper.Map<UserDto>(u))
    //        .ToList();
    //}
    public async Task<List<UserDto>> GetTeachersAsync()
    {
        var teachers = await _userRepository.GetByRoleAsync("Teacher");
        return teachers.Select(u => _mapper.Map<UserDto>(u)).ToList();
    }

    public async Task<UserDto> CreateAsync(UserCreateDto dto)
    {
        var user = _mapper.Map<User>(dto);
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> UpdateAsync(UserUpdateDto dto)
    {
        var existing = await _userRepository.GetByIdAsync(dto.UserId);
        if (existing == null) return null;

        _mapper.Map(dto, existing);
        await _userRepository.SaveChangesAsync();

        return _mapper.Map<UserDto>(existing);
    }
    public async Task<UserDto?> ChangeRoleAsync(int id, string newRole)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        // chỉ cho phép đổi giữa User và Teacher
        if (newRole != "User" && newRole != "Teacher")
            throw new ArgumentException("Invalid role. Allowed: User, Teacher");

        user.Role = newRole;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.SaveChangesAsync();
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> BanAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        user.Status = "Banned";
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }
    public async Task<UserDto?> UnbanAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        user.Status = "Active";
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

}