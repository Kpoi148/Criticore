using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Identity.Api.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet("auth/google-signin")]
        public IActionResult GoogleSignIn()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/class-list" // hoặc redirect frontend
            }, GoogleDefaults.AuthenticationScheme);
        }

    }
}
