using Front_end.Pages.Admin;
using Front_end.Services.Interfaces;
using Front_end.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;  // Thêm cho CookieOptions

namespace Front_end
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRazorPages();
            builder.Services.AddHttpClient();

            builder.Services.AddHttpClient<ITopicService, TopicService>();
            builder.Services.AddHttpClient<ITopicDetailService, TopicDetailService>();
            builder.Services.AddHttpClient("GatewayClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7215");
            });
            builder.Services.AddHttpClient<IClassesService, ClassesService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7193/");
            });
            builder.Services.AddHttpClient<IMaterialService, MaterialService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7179/");
            });
            builder.Services.AddHttpClient<IUsersService, UsersService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7281/");
            });
            builder.Services.AddHttpClient<IHomeworkService, HomeworkService>();
            // Config JWT
            var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? "your-super-secret-key-that-is-at-least-32-chars-long-abc123");
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "YourApp",
                        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "YourAppUsers",
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var token = context.Request.Cookies["authToken"];  // Ưu tiên cookie
                            if (!string.IsNullOrEmpty(token))
                            {
                                context.Token = token;
                            }
                            else if (context.Request.Query.ContainsKey("token"))
                            {
                                token = context.Request.Query["token"].ToString();
                                context.Token = token;
                                // Lưu vào cookie để reload vẫn có
                                context.Response.Cookies.Append("authToken", token, new CookieOptions
                                {
                                    HttpOnly = true,  // Bảo mật, JS không đọc được
                                    Secure = false,   // Dev: false; Prod: true
                                    SameSite = SameSiteMode.Lax,                                  
                                });
                            }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"Auth fail: {context.Exception.Message}");  // Log debug
                            return Task.CompletedTask;
                        }
                    };
                });

            var app = builder.Build();
            app.MapPost("/api/logout", (HttpContext context) =>
            {
                // Xóa cookie chứa JWT
                context.Response.Cookies.Delete("authToken");
                return Results.Ok(new { message = "Logged out" });
            });
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapRazorPages();
            app.Run();
        }
    }
}