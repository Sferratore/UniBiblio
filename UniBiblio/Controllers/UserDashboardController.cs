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
            var libri = await _context.Prenotalibriviews
                .Where(l => l.QuantitaDisponibile > 0)
                .Select(l => new Prenotalibriview
                {
                    IdLibro = l.IdLibro,
                    Titolo = l.Titolo,
                    Autore = l.Autore,
                    AnnoPubblicazione = l.AnnoPubblicazione,
                    Isbn = l.Isbn,
                    Categoria = l.Categoria,
                    Biblioteca = l.Biblioteca,
                    QuantitaDisponibile = (int)l.QuantitaDisponibile
                })
                .ToListAsync();

            return View(libri);
        }


        [HttpPost]
        public async Task<IActionResult> EffettuaPrenotazione(int id_libro)
        {
            // Recupera l'email dell'utente dalla sessione
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            // Recupera l'utente dal database
            var utente = await _context.Utentis.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (utente == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Recupera il libro
            var libro = await _context.Libris.FirstOrDefaultAsync(l => (int)l.IdLibro == id_libro);
            if (libro == null || libro.QuantitaDisponibile <= 0)
            {
                ModelState.AddModelError(string.Empty, "Il libro selezionato non è disponibile.");
                return RedirectToAction("PrenotaLibri", "UserDashboard");
            }

            // Crea una nuova prenotazione
            var nuovaPrenotazione = new PrenotazioniLibri
            {
                IdUtente = (int)utente.IdUtente,
                IdLibro = (int)libro.IdLibro,
                DataPrenotazione = DateOnly.FromDateTime(DateTime.Now),
                Stato = "Prenotato"
            };

            // Aggiungi la prenotazione al database
            _context.PrenotazioniLibris.Add(nuovaPrenotazione);

            // Riduci la quantità disponibile del libro
            libro.QuantitaDisponibile--;

            // Salva le modifiche
            await _context.SaveChangesAsync();

            // Redirige a una pagina di conferma o alla lista delle prenotazioni
            return RedirectToAction("PrenotaLibri", "UserDashboard");

        }
    }
}
