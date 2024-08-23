using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniBiblio.Models;

namespace UniBiblio.Controllers
{
    public class UserDashboardController : Controller
    {

        private readonly unibiblioContext _context;

        public UserDashboardController(unibiblioContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Home()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PrenotaLibri()
        {
            var libri = await _context.Libris
                .Where(l => l.QuantitaDisponibile > 0)
                .Select(l => new Libri
                {
                    IdLibro = l.IdLibro,
                    Titolo = l.Titolo,
                    Autore = l.Autore,
                    AnnoPubblicazione = l.AnnoPubblicazione,
                    Isbn = l.Isbn,
                    Categoria = l.Categoria,
                    QuantitaDisponibile = (int)l.QuantitaDisponibile
                })
                .ToListAsync();

            return View(libri);
        }
    }
}
