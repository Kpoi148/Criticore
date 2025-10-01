
using Microsoft.EntityFrameworkCore;
using TopicDetail.Infrastructure.Models;
using TopicDetail.Domain.Repositories;
using TopicDetail.Infrastructure.Repositories;
using TopicDetail.Application.Services;
using TopicDetail.Application.Profiles;

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
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy => policy.WithOrigins("https://localhost:7186")
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
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


            app.MapControllers();

            app.Run();
        }
    }
}
