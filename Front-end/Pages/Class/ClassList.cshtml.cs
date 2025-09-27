using Microsoft.AspNetCore.Authorization;  // Thêm
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DomainClass = Class.Domain.Entities.Class;
using Class.Domain.Entities;

namespace Front_end.Pages.Class
{
    [Authorize]  // Thêm: Tự redirect nếu không auth
    public class ClassListModel : PageModel
    {
        public List<DomainClass> Classes { get; set; } = new List<DomainClass>();
        public List<User> Students { get; set; } = new List<User>();
        public string? CurrentUserId { get; set; }

        public void OnGet()
        {
            // Lấy userId từ claims (bây giờ sẽ có nếu token valid)
            CurrentUserId = User.FindFirst("UserId")?.Value;
            Console.WriteLine($"UserId: {CurrentUserId}");  // Log test
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                // Không redirect ngay, redirect với status 302
                HttpContext.Response.Redirect("/Signin");
                HttpContext.Response.StatusCode = 302;
                return;
            }

            // Hardcode data
            Students = new List<User>
            {
                new() { UserId = 1, FullName = "Student1" },
                new() { UserId = 2, FullName = "Student2" },
                new() { UserId = 3, FullName = "Student3" }
            };
            Classes = new List<DomainClass>
            {
                // Uncomment nếu cần demo
                // new() { ... }
            };
        }
    }
}