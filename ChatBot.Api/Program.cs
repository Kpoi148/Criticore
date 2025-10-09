using ChatBot.Application.Services;
using ChatBot.Application.Services.Interfaces;
using ChatBot.Infrastructure.Client;
using ChatBot.Infrastructure.Client.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChatBot.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient();
            // 🟢 Lấy API key từ biến môi trường thay vì appsettings.json
            var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (string.IsNullOrEmpty(openAiApiKey))
            {
                Console.WriteLine("⚠️ WARNING: OPENAI_API_KEY environment variable is not set!");
            }
            // 🟢 Đăng ký HttpClient cho OpenAiClient
            builder.Services.AddHttpClient<IOpenAiClient, OpenAiClient>(client =>
            {
                client.BaseAddress = new Uri("https://api.openai.com/v1/");
                if (!string.IsNullOrEmpty(openAiApiKey))
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAiApiKey}");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            // Các service khác
            builder.Services.AddScoped<IAiRAGProxy, AiRAGProxy>();
            builder.Services.AddScoped<IAiInteractionService, AiInteractionService>();
            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
            // Session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            // Logging
            builder.Services.AddLogging();
            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseCors();
            app.UseSession();
            app.MapControllers();

            // 🔴 Thêm route default cho "/" để fix 404 (test hoặc redirect đến Swagger)
            app.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("ChatBot API is running! Access Swagger at /swagger for endpoints.");
            });

            app.Run();
        }
    }
}