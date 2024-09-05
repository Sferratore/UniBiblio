using Microsoft.AspNetCore.Mvc;

namespace UniBiblio.Controllers
{
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
