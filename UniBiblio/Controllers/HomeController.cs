using Microsoft.AspNetCore.Mvc;

namespace UniBiblio.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
