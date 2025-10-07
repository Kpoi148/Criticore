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

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpClient();

            // Đăng ký OpenAiClient với Scoped (khớp constructor nhận IHttpClientFactory và IConfiguration)
            builder.Services.AddHttpClient<IOpenAiClient, OpenAiClient>(client =>
            {
                client.BaseAddress = new Uri("https://api.openai.com/v1/");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {builder.Configuration["OpenAi:ApiKey"]}");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });


            // Đăng ký HttpClient cho IAiRAGProxy (giữ nguyên)
            builder.Services.AddScoped<IAiRAGProxy, AiRAGProxy>();

            builder.Services.AddScoped<IAiInteractionService, AiInteractionService>();

            // Thêm CORS để tránh lỗi cross-origin
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            // Nếu dùng session (như trong controller), thêm đây
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Thêm logging để debug (tùy chọn, nhưng hữu ích)
            builder.Services.AddLogging();

            // Nếu dùng auth (JWT hoặc Identity), thêm đây. Nếu không, xóa UseAuthorization() ở dưới
            // builder.Services.AddAuthentication("Scheme").AddJwtBearer(options => { /* config */ });
            // builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors(); // CORS trước auth
            app.UseSession(); // Nếu dùng session

            // Nếu không dùng auth, xóa dòng này để tránh lỗi
            // app.UseAuthorization();

            app.MapControllers(); // Map API controllers

            app.Run();
        }
    }
}