
using Homework.Application.Profiles;
using Homework.Application.Services;
using Homework.Domain.Repositories;
using Homework.Infrastructure.Models;
using Homework.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Homework.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Trong Program.cs (hoặc Startup.cs)
            builder.Services.AddControllers()
                .AddNewtonsoftJson(); // Thêm dòng này vào
            // Đăng ký DbContext
            builder.Services.AddDbContext<HomeworkDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<IHomeworkRepository, HomeworkRepository>();
            builder.Services.AddScoped<HomeworkService>();
            builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
            builder.Services.AddScoped<ICopyleaksReportRepository, CopyleaksReportRepository>();
            builder.Services.AddScoped<SubmissionService>();

            builder.Services.AddHttpClient<CopyleaksService>();
            builder.Services.AddScoped<CopyleaksService>();

            builder.Services.AddScoped<CopyleaksWebhookService>();
            builder.Services.AddScoped<CopyleaksReportService>();

            builder.Services.AddAutoMapper(typeof(HomeworkProfile).Assembly);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy => policy.WithOrigins("https://localhost:7186")
                                    //.WithOrigins("https://criticore.edu.vn:8386")
                                    .AllowAnyMethod()
                                    .AllowAnyHeader()
                                    .AllowCredentials());
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowFrontend");
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
