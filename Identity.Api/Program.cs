using Identity.Application.Services;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Models;
using Identity.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


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

            // Đăng ký DbContext
            builder.Services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Đăng ký Repository và Service
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<UserService>();
            //
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();


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

                options.Events.OnTicketReceived = async context =>
                {
                    var userService = context.HttpContext.RequestServices.GetRequiredService<UserService>();
                    var user = await userService.CreateOrGetUserFromGoogleAsync(context.Principal);

                    // Tạo JWT
                    var jwtHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(context.HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Key"]);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[]
                        {
                new Claim("UserId", user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
                        }),
                        Expires = DateTime.UtcNow.AddMinutes(double.Parse(context.HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:ExpireMinutes"])),
                        Issuer = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Issuer"],
                        Audience = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Audience"],
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = jwtHandler.CreateToken(tokenDescriptor);
                    var jwtToken = jwtHandler.WriteToken(token);

                    // Trả JWT cho FE, ví dụ redirect kèm query param
                    var redirectUri = $"https://localhost:7186/Class/ClassList?token={jwtToken}";
                    context.Response.Redirect(redirectUri);
                    context.HandleResponse(); // dừng cookie
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
            app.MapControllers();
            app.Run();
        }
    }
}