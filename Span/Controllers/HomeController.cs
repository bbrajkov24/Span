using Microsoft.AspNetCore.Mvc;

namespace Span.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
