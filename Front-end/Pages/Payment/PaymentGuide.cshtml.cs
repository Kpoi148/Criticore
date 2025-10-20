using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Front_end.Pages.Payment
{
    public class PaymentGuideModel : PageModel
    {
        [TempData]
        public string? SuccessMessage { get; set; }
        public void OnGet()
        {
        }
    }
}
