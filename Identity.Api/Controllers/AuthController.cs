using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;


namespace Identity.Api.Controllers
{
    public class AuthController : ControllerBase
    {
        [HttpGet("auth/google-signin")]
        public IActionResult GoogleSignIn()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "https://localhost:7186/Class/ClassList"  // Redirect sau auth thành công
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("test-session")]
        public IActionResult TestSession()
        {
            if (HttpContext.User.Identity?.IsAuthenticated == true)
            {
                var userId = HttpContext.User.FindFirst("UserId")?.Value;
                return Ok(new { IsAuthenticated = true, UserId = userId });
            }
            return Ok(new { IsAuthenticated = false });
        }
    }
}