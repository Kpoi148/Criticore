using Identity.Api.Hubs;
using Identity.Application.Profiles;
using Identity.Application.Services;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Models;
using Identity.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
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
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.Secure = CookieSecurePolicy.Always;
            });

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
                options.CorrelationCookie.SameSite = SameSiteMode.None;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

                options.Events.OnTicketReceived = async context =>
                {
                    try
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
            new Claim(ClaimTypes.Role, user.Role ?? "User") // ✅ thêm role
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
                        //? $"https://criticore.edu.vn:8386/adminclasses/index?token={jwtToken}"
                        //    : $"https://criticore.edu.vn:8386/Class/ClassList?token={jwtToken}";

                        context.Response.Redirect(redirectUri);
                        context.HandleResponse(); // Dừng xử lý cookie
                    }
                    catch (Exception ex)
                    {
                        // Log lỗi (sử dụng ILogger nếu có, hoặc console)
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>(); // Giả sử bạn có ILogger<Program>
                        logger.LogError(ex, "Lỗi trong OnTicketReceived: {Message}", ex.Message);

                        // Xử lý response: Redirect về trang lỗi hoặc trả về 500
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("Đăng nhập thất bại. Vui lòng thử lại sau.");
                        context.HandleResponse(); // Dừng xử lý thêm
                    }
                };

            });


    //        builder.Services.AddDataProtection()
    //.PersistKeysToFileSystem(new DirectoryInfo(@"C:\keys"))
    //.ProtectKeysWithDpapi(protectToLocalMachine: true);  // Thêm protectToLocalMachine

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapHub<UserHub>("/userHub");
            app.UseCookiePolicy();
            app.MapControllers();
            app.Run();
        }
    }
}