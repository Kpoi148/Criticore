
using Material.Application.Services;
using Material.Domain.Repositories;
using Material.Infrastructure.Models;
using Material.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Material.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Đăng ký DbContext
            builder.Services.AddDbContext<MaterialContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("VPSConnection")));

            // Đăng ký CloudinaryService (singleton, vì config cố định)
            builder.Services.AddSingleton<CloudinaryService>();
            // Đăng ký MaterialService (scoped, mỗi request 1 instance)
            builder.Services.AddScoped<MaterialService>();
            // Đăng ký Repository
            builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
          
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

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
