using ChatBot.Application.Services;
using ChatBot.Application.Services.Interfaces;
using ChatBot.Infrastructure.Client;
using ChatBot.Infrastructure.Client.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChatBot.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Thêm logging để debug
            builder.Logging.AddConsole();
            // Add services
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // 🟢 CORS - chỉ khai báo 1 lần thôi
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                    policy
                        .WithOrigins("https://criticore.edu.vn:8386") // Origin chính xác
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                );
            });
            // 🧠 Lấy API key: Ưu tiên environment variable, fallback vào appsettings.json
            var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? builder.Configuration["OpenAI:ApiKey"];  // Lấy từ appsettings.json nếu env không có

            if (string.IsNullOrEmpty(openAiApiKey))
            {
                Console.WriteLine("⚠️ WARNING: OPENAI_API_KEY không được set ở environment hoặc appsettings.json!");
                // Hoặc throw exception nếu muốn dừng app: throw new Exception("Missing OPENAI_API_KEY");
            }
            // 🧠 HttpClient cho OpenAI
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
            // Session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Logging.AddFilter("Microsoft.AspNetCore.Cors", LogLevel.Debug);
            builder.Logging.AddFilter("Microsoft.AspNetCore.Hosting", LogLevel.Debug);
            var app = builder.Build();
            // Thêm exception handling để capture lỗi 500
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Hiển thị stack trace chi tiết trên trình duyệt
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler(exceptionHandlerApp =>
                {
                    exceptionHandlerApp.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/plain";
                        var exceptionFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
                        if (exceptionFeature != null)
                        {
                            // Log lỗi chi tiết
                            app.Logger.LogError(exceptionFeature.Error,
                                "Lỗi 500 tại path {Path}, TraceId: {TraceId}, Message: {Message}",
                                exceptionFeature.Path, context.TraceIdentifier, exceptionFeature.Error.Message);
                            await context.Response.WriteAsync($"Lỗi nội bộ: {exceptionFeature.Error.Message}. Kiểm tra log để biết chi tiết.");
                        }
                        else
                        {
                            await context.Response.WriteAsync("Lỗi nội bộ không xác định. Kiểm tra log.");
                        }
                    });
                });
            }
            // ✅ Phải có thứ tự như sau
            //app.UseHttpsRedirection();
            app.UseRouting(); // 🚨 Quan trọng: phải có trước CORS
            app.UseCors("AllowFrontend"); // ✅ CORS ở giữa Routing và MapControllers
            app.UseSession();
            app.UseAuthorization();
            app.Use(async (context, next) =>
            {
                if (context.Request.Method == "OPTIONS")
                {
                    context.Response.StatusCode = 200;
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "https://criticore.edu.vn:8386");
                    context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                    context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                    await context.Response.CompleteAsync();
                    return;
                }
                await next();
            });
            app.MapControllers();
            // Thêm endpoint test để debug CORS/OPTIONS
            app.Map("/test-cors", () => "CORS Test OK").RequireCors("AllowFrontend");
            app.Run();
        }
    }
}