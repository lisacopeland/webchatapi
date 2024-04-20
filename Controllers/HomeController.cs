using Microsoft.AspNetCore.Mvc;

namespace webchat.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
