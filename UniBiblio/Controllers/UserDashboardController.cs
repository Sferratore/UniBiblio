﻿using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using UniBiblio.Models;

namespace UniBiblio.Controllers
{
    public class UserDashboardController : Controller
    {

        private readonly UniBiblioContext _context;
        private readonly string _connectionString;

        public UserDashboardController(UniBiblioContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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
        public async Task<IActionResult> PrenotaLibri(int id_libro)
        {
            // Recupera l'email dell'utente dalla sessione
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["ErrorMessage"] = "È necessario effettuare il login per prenotare un libro.";
                return RedirectToAction("Login", "Account");
            }

            // Recupera l'utente dal database
            var utente = await _context.Utentis.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (utente == null)
            {
                TempData["ErrorMessage"] = "L'utente in sessione non corrisponde a nessun utente nel DB. Contatta un amministratore.";
                return RedirectToAction("Login", "Account");
            }

            // Controlla il numero di prenotazioni non ritirate
            int prenotazioniNonRitirate = await _context.Prenotazionieffettuatelibriviews
                .CountAsync(p => p.IdUtente == (int)utente.IdUtente && p.DataRitiro == null);

            if (prenotazioniNonRitirate >= 3)
            {
                TempData["ErrorMessage"] = "Hai già 3 prenotazioni non ritirate. Non puoi effettuare ulteriori prenotazioni.";
                return RedirectToAction("PrenotaLibri", "UserDashboard");
            }

            // Recupera il libro
            var libro = await _context.Libris.FirstOrDefaultAsync(l => (int)l.IdLibro == id_libro);
            if (libro == null || libro.QuantitaDisponibile <= 0)
            {
                TempData["ErrorMessage"] = "Il libro selezionato non è disponibile.";
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
            TempData["Message"] = "Prenotazione EFFETTUATA correttamente!";
            return RedirectToAction("PrenotaLibri", "UserDashboard");

        }

        [HttpGet]
        public async Task<IActionResult> PrenotaSale(DateTime date)
        {
            try
            {
                //Qui usiamo Dapper perché EFC non ci permette di ottenere risultati dal DB se non si tratta di tabelle o view che comunicano col dbcontext
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var sale = await connection.QueryAsync<PrenotaSaleProcedureRecord>(
                        "CALL GetAvailableSeatsByDate(@Date)",
                        new { Date = date }
                    );
                    return View(sale);
                }

            }
            catch (Exception ex)
            {
                return Content("Errore nell'esecuzione della procedura per ottenere i posti disponibili!");
            }


        }



        //[HttpPost]
        //public async Task<IActionResult> PrenotaSale(int id_sala)
        //{
        //    // Recupera l'email dell'utente dalla sessione
        //    var userEmail = HttpContext.Session.GetString("UserEmail");

        //    if (string.IsNullOrEmpty(userEmail))
        //    {
        //        TempData["ErrorMessage"] = "È necessario effettuare il login per prenotare una sala.";
        //        return RedirectToAction("Login", "Account");
        //    }

        //    // Recupera l'utente dal database
        //    var utente = await _context.Utentis.FirstOrDefaultAsync(u => u.Email == userEmail);
        //    if (utente == null)
        //    {
        //        TempData["ErrorMessage"] = "L'utente in sessione non corrisponde a nessun utente nel DB. Contatta un amministratore.";
        //        return RedirectToAction("Login", "Account");
        //    }

        //    // Controlla il numero di prenotazioni non ritirate
        //    int prenotazioniNonRitirate = await _context.Prenotazionieffettuatelibriviews
        //        .CountAsync(p => p.IdUtente == (int)utente.IdUtente && p.DataRitiro == null);

        //    if (prenotazioniNonRitirate >= 3)
        //    {
        //        TempData["ErrorMessage"] = "Hai già 3 prenotazioni non ritirate. Non puoi effettuare ulteriori prenotazioni.";
        //        return RedirectToAction("PrenotaLibri", "UserDashboard");
        //    }

        //    // Recupera il libro
        //    var libro = await _context.Libris.FirstOrDefaultAsync(l => (int)l.IdLibro == id_libro);
        //    if (libro == null || libro.QuantitaDisponibile <= 0)
        //    {
        //        TempData["ErrorMessage"] = "Il libro selezionato non è disponibile.";
        //        return RedirectToAction("PrenotaLibri", "UserDashboard");
        //    }

        //    // Crea una nuova prenotazione
        //    var nuovaPrenotazione = new PrenotazioniLibri
        //    {
        //        IdUtente = (int)utente.IdUtente,
        //        IdLibro = (int)libro.IdLibro,
        //        DataPrenotazione = DateOnly.FromDateTime(DateTime.Now),
        //        Stato = "Prenotato"
        //    };

        //    // Aggiungi la prenotazione al database
        //    _context.PrenotazioniLibris.Add(nuovaPrenotazione);

        //    // Riduci la quantità disponibile del libro
        //    libro.QuantitaDisponibile--;

        //    // Salva le modifiche
        //    await _context.SaveChangesAsync();

        //    // Redirige a una pagina di conferma o alla lista delle prenotazioni
        //    TempData["Message"] = "Prenotazione EFFETTUATA correttamente!";
        //    return RedirectToAction("PrenotaLibri", "UserDashboard");

        //}
    }


}
