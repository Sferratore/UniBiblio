using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniBiblio.Models;

namespace UniBiblio.Controllers
{
    public class AccountController : Controller
    {
        private readonly UniBiblioContext _context;

        public AccountController(UniBiblioContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Verifica dell'utente
            var utente = await _context.Utentis.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (utente != null && BCrypt.Net.BCrypt.Verify(model.Password, utente.PasswordHash))
            {
                // Logica per l'utente loggato
                HttpContext.Session.SetString("UserEmail", utente.Email);
                HttpContext.Session.SetString("IsAdmin", utente.IsAmministratore == true ? "true" : "false");
                if (HttpContext.Session.GetString("IsAdmin") == "true")
                {
                    return RedirectToAction("Index", "AdminDashboard");
                }
                return RedirectToAction("Home", "UserDashboard");
            }

            //Logica per i login falliti
            TempData["ErrorMessage"] = "Credenziali non valide.";
            return View(model);

        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Controllo se l'email esiste già
            var existingUser = await _context.Utentis.AnyAsync(u => u.Email == model.Email);
            if (existingUser)
            {
                TempData["ErrorMessage"] = "Un utente con questa mail è già registrato.";
                return View(model);
            }

            // Creazione dell'utente
            var nuovoUtente = new Utenti
            {
                Nome = model.Nome,
                Cognome = model.Cognome,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
            };

            _context.Utentis.Add(nuovoUtente);
            await _context.SaveChangesAsync();


            TempData["Message"] = "Registrazione avvenuta con successo!";
            return RedirectToAction("Login", "Account");
        }
    }
}