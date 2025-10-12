using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Front_end.Pages
{
    public class SignInModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public SignInModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult OnGetGoogle()
        {
            string identityUrl = _configuration["Identity:Url"] ?? "https://identity.criticore.edu.vn:8004"; // Lấy từ config để dễ thay đổi
            return Redirect($"{identityUrl}/auth/google-signin");
        }
    }
}   