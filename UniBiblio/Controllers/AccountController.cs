using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniBiblio.Models;

namespace UniBiblio.Controllers
{
    public class AccountController : Controller
    {
        private readonly unibiblioContext _context;

        public AccountController(unibiblioContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
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
                return RedirectToAction("UserDashboard", "Home");
            }
            else
            {
                if (utente == null)
                {
                    // Se non è un utente, controlla se è un amministratore
                    var amministratore = await _context.Amministratoris
                        .FirstOrDefaultAsync(a => a.Email == model.Email);

                    if (amministratore != null && BCrypt.Net.BCrypt.Verify(model.Password, amministratore.PasswordHash))
                    {
                        // Logica per l'amministratore loggato
                        return RedirectToAction("AdminDashboard", "Home");
                    }


                }
            }

            //Logica per i login falliti
            ModelState.AddModelError(string.Empty, "Credenziali non valide.");
            return View(model);

        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
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
                ModelState.AddModelError(string.Empty, "Un utente con questa email è già registrato.");
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

            return RedirectToAction("Login", "Account");
        }
    }
}