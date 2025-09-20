using Ocelot.DependencyInjection;  // Cho AddOcelot
using Ocelot.Middleware;  // Cho UseOcelot
namespace OcelotApiGateway
{
    public class Program
    {
        public static async Task Main(string[] args)  // Thêm async nếu cần
        {
            var builder = WebApplication.CreateBuilder(args);

            // Load ocelot.json
            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

            // Add Ocelot services
            builder.Services.AddOcelot(builder.Configuration);

            // Add services to the container (giữ nguyên nếu cần)
            builder.Services.AddControllers();
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
            // app.UseAuthorization();  // Comment nếu không cần auth, để tránh chặn request

            app.MapControllers();

            await app.UseOcelot();  // Thêm middleware Ocelot

            app.Run();
        }
    }
}