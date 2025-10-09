using Identity.Api.Hubs;
using Identity.Application.Profiles;
using Identity.Application.Services;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Models;
using Identity.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;


namespace Identity.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR();

            // Đăng ký DbContext
            builder.Services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("LocConnection")));

            // Đăng ký Repository và Service
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<UserService>();
            //
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            // Auto mapper
            builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);

            var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            })
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                options.CallbackPath = "/auth/callback";
                options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");

                options.Events.OnTicketReceived = async context =>
                {
                    var userService = context.HttpContext.RequestServices.GetRequiredService<UserService>();
                    var user = await userService.CreateOrGetUserFromGoogleAsync(context.Principal);

                    // Tạo JWT
                    var jwtHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(context.HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Key"]);

                    var claims = new List<Claim>
                    {
                        new Claim("UserId", user.UserId.ToString()),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role ?? "User")   // ✅ thêm role
                    };

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.UtcNow.AddMinutes(
                            double.Parse(context.HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:ExpireMinutes"])),
                        Issuer = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Issuer"],
                        Audience = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Audience"],
                        SigningCredentials = new SigningCredentials(
                            new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = jwtHandler.CreateToken(tokenDescriptor);
                    var jwtToken = jwtHandler.WriteToken(token);

                    // Nếu là Admin thì redirect sang admin page, còn không thì về ClassList
                    var redirectUri = user.Role == "Admin"
                        ? $"https://localhost:7186/adminclasses/index?token={jwtToken}"
                        : $"https://localhost:7186/Class/ClassList?token={jwtToken}";

                    context.Response.Redirect(redirectUri);
                    context.HandleResponse(); // Dừng xử lý cookie
                };

            });

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapHub<UserHub>("/userHub");
            app.MapControllers();
            app.Run();
        }
    }
}