using Front_end.Pages.Admin;

namespace Front_end
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            // Đăng ký HttpClient
            builder.Services.AddHttpClient();
            // Đăng ký Named HttpClient
            builder.Services.AddHttpClient("GatewayClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7215"); // thay bằng API gateway hoặc backend
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline. 
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}