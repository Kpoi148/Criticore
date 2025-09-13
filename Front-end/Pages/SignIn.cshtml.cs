using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Front_end.Pages
{
    public class SignInModel : PageModel
    {
        public IActionResult OnGetGoogle()
        {
            return Redirect("http://localhost:5287/auth/google-signin");// Gọi qua Ocelot
        }
    }
}
