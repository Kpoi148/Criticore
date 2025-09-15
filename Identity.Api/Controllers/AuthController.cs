using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Application.Services;  // Using cho UserService

namespace Identity.Api.Controllers
{
    public class AuthController : ControllerBase
    {
        [HttpGet("auth/google-signin")]
        public IActionResult GoogleSignIn()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/class-list"  // Redirect sau auth thành công
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
    }
}