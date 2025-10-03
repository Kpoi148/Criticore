
using Microsoft.EntityFrameworkCore;
using TopicDetail.Api.Hubs;
using TopicDetail.Application.Profiles;
using TopicDetail.Application.Services;
using TopicDetail.Domain.Repositories;
using TopicDetail.Infrastructure.Models;
using TopicDetail.Infrastructure.Repositories;

namespace TopicDetail.Api
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
            // Thêm dịch vụ SignalR
            builder.Services.AddSignalR();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy => policy.WithOrigins("https://localhost:7186")
                                    .AllowAnyMethod()
                                    .AllowAnyHeader()
                                    .AllowCredentials());
            });
            builder.Services.AddAutoMapper(typeof(TopicDetailMapping).Assembly);
            // Đăng ký DbContext
            builder.Services.AddDbContext<TopicDetailDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Đăng ký Repository và Service
            builder.Services.AddScoped<ITopicDetailRepository,TopicDetailRepository>();
            builder.Services.AddScoped<TopicDetailService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
            app.UseAuthorization();

            // Map endpoint cho Hub
            app.MapHub<TopicHub>("/topicHub");  // Endpoint: ws://localhost:port/topicHub
            app.MapControllers();

            app.Run();
        }
    }
}
