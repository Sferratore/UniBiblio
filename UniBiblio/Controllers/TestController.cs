using Microsoft.AspNetCore.Mvc;
using UniBiblio.Models;

namespace UniBiblio.Controllers
{
    public class TestController : Controller
    {
        private readonly unibiblioContext _context;

        public TestController(unibiblioContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            bool hasRecords = _context.Amministratoris.Any();
            ViewBag.HasRecords = hasRecords;
            return View();
        }
    }
}
