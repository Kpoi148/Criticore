using Microsoft.AspNetCore.Mvc;

namespace Front_end.Controllers
{
    public class ClassController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }        
    }
}
