
using Class.Application.Services;
using Class.Domain.Repositories;
using Class.Infrastructure.Models;
using Class.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Class.API
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
            builder.Services.AddDbContext<ClassDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            Console.WriteLine("👉 Connection string DefaultConnection: "
     + builder.Configuration.GetConnectionString("DefaultConnection"));
            Console.WriteLine("👉 Connection string LocConnection: "
                + builder.Configuration.GetConnectionString("LocConnection"));



            // Đăng ký Repository và Service
            builder.Services.AddScoped<IClassRepository, ClassRepository>();
            builder.Services.AddScoped<ClassService>();
            builder.Services.AddScoped<JoinRequestService>();
            builder.Services.AddScoped<IJoinRequestRepository, JoinRequestRepository>();
            builder.Services.AddScoped<IClassMemberRepository, ClassMemberRepository>();

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
