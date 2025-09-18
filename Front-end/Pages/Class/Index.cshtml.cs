using Front_end.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Front_end.Pages.Class
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _http;
        public IndexModel(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
            _http.BaseAddress = new Uri("https://localhost:5001/");
        }

        public List<ClassDto> Classes { get; set; } = new();

        public async Task OnGetAsync()
        {
            var result = await _http.GetFromJsonAsync<List<ClassDto>>("api/classes");
            if (result != null)
                Classes = result;
        }
    }
}
