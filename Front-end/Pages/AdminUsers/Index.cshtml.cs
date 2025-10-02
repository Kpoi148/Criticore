using Front_end.DTOs;
using Front_end.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Front_end.Pages.AdminUsers
{
    public class IndexModel : PageModel
    {
        private readonly IUsersService _usersService;
        public List<UserDto> Users { get; set; } = new();

        public IndexModel(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public async Task OnGetAsync()
        {
            Users = await _usersService.GetAllAsync();
        }

        public async Task OnPostBanAsync(int id)
        {
            await _usersService.BanAsync(id);
            Users = await _usersService.GetAllAsync();
        }

        public async Task OnPostUnbanAsync(int id)
        {
            await _usersService.UnbanAsync(id);
            Users = await _usersService.GetAllAsync();
        }

        public async Task OnPostChangeRoleAsync(int id, int roleType)
        {
            await _usersService.ChangeRoleAsync(id, roleType);
            Users = await _usersService.GetAllAsync();
        }
    }
}
