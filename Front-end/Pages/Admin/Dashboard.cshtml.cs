using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Class.Domain.Entities; // Để dùng Class
using DomainClass = Class.Domain.Entities.Class;
using Class.Domain.DTOs; // Nếu cần DTO

namespace Front_end.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DashboardModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GatewayClient"); // Config base ở Program.cs: AddHttpClient("GatewayClient", c => c.BaseAddress = new Uri("http://gateway-url"));
        }

        public List<DomainClass> Classes { get; set; } = new List<DomainClass>();
        public List<User> Teachers { get; set; } = new List<User>(); // Giả định User từ domain

        public async Task OnGetAsync()
        {
            // Gọi API Classes qua gateway
            var responseClasses = await _httpClient.GetAsync("/classes/Classes");
            Console.WriteLine("Classes status: " + responseClasses.StatusCode);
            if (responseClasses.IsSuccessStatusCode)
            {
                var json = await responseClasses.Content.ReadAsStringAsync();
                Classes = JsonSerializer.Deserialize<List<DomainClass>>(json) ?? new List<DomainClass>();
            }

            // Gọi API Teachers (giả định từ identity service)
            //var responseTeachers = await _httpClient.GetAsync("/identity/Users/teachers"); // Điều chỉnh endpoint nếu khác
            //if (responseTeachers.IsSuccessStatusCode)
            //{
            //    var json = await responseTeachers.Content.ReadAsStringAsync();
            //    Teachers = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            //}
        }
    }
}