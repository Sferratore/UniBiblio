using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MySqlConnector;
using OfficeOpenXml;
using UniBiblio.Models;


namespace UniBiblio.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly UniBiblioContext _context;
        private readonly string _connectionString;

        public AdminDashboardController(UniBiblioContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            var libriReservations = _context.Prenotazionieffettuatelibriviews.ToList();
            var saleReservations = _context.Prenotazionieffettuatesaleviews.ToList();

            var model = new AdminDashboardViewModel
            {
                LibriReservations = libriReservations,
                SaleReservations = saleReservations
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Analytics()
        {

            try
            {
                // Imposta il contesto di licenza EPPlus
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Qui usiamo Dapper perché EFC non ci permette di ottenere risultati dal DB se non si tratta di tabelle o view che comunicano col dbcontext
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var sale = await connection.ExecuteAsync(
                        "CALL InsertMonthlyStatistics()"
                    );

                    var statistiche = await _context.StatistichePrenotazioniMensilis.ToListAsync();

                    // Utilizza un MemoryStream per creare il file Excel in memoria
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var package = new ExcelPackage(memoryStream))
                        {
                            // Aggiungi un foglio di lavoro
                            var worksheet = package.Workbook.Worksheets.Add("Statistiche Prenotazioni Mensili");

                            // Aggiungi intestazioni (nomi delle colonne)
                            worksheet.Cells[1, 1].Value = "Mese";
                            worksheet.Cells[1, 2].Value = "Anno";
                            worksheet.Cells[1, 3].Value = "Totale Prenotazioni Libri";
                            worksheet.Cells[1, 4].Value = "Totale Prenotazioni Sale";
                            worksheet.Cells[1, 5].Value = "Prenotazioni Libri Prenotati";
                            worksheet.Cells[1, 6].Value = "Prenotazioni Libri Ritirati";
                            worksheet.Cells[1, 7].Value = "Prenotazioni Libri Restituiti";
                            worksheet.Cells[1, 8].Value = "Prenotazioni Libri Cancellati";
                            worksheet.Cells[1, 9].Value = "Prenotazioni Sale Prenotate";
                            worksheet.Cells[1, 10].Value = "Prenotazioni Sale Usufruite";
                            worksheet.Cells[1, 11].Value = "Prenotazioni Sale Cancellate";
                            worksheet.Cells[1, 12].Value = "Libro Più Prenotato";
                            worksheet.Cells[1, 13].Value = "Sala Più Prenotata";

                            // Aggiungi i dati delle statistiche
                            int row = 2;
                            foreach (var statistica in statistiche)
                            {
                                worksheet.Cells[row, 1].Value = statistica.Mese;
                                worksheet.Cells[row, 2].Value = statistica.Anno;
                                worksheet.Cells[row, 3].Value = statistica.TotalePrenotazioniLibri;
                                worksheet.Cells[row, 4].Value = statistica.TotalePrenotazioniSale;
                                worksheet.Cells[row, 5].Value = statistica.PrenotazioniLibriPrenotati;
                                worksheet.Cells[row, 6].Value = statistica.PrenotazioniLibriRitirati;
                                worksheet.Cells[row, 7].Value = statistica.PrenotazioniLibriRestituiti;
                                worksheet.Cells[row, 8].Value = statistica.PrenotazioniLibriCancellati;
                                worksheet.Cells[row, 9].Value = statistica.PrenotazioniSalePrenotate;
                                worksheet.Cells[row, 10].Value = statistica.PrenotazioniSaleUsufruite;
                                worksheet.Cells[row, 11].Value = statistica.PrenotazioniSaleCancellate;
                                worksheet.Cells[row, 12].Value = statistica.LibroPiuPrenotato;
                                worksheet.Cells[row, 13].Value = statistica.SalaPiuPrenotata;
                                row++;
                            }

                            // Formatta il foglio di lavoro (facoltativo)
                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                            worksheet.Cells[1, 1, 1, 13].Style.Font.Bold = true;

                            // Salva il pacchetto Excel nel memory stream
                            package.Save();
                        }

                        // Riposiziona il memoryStream all'inizio
                        memoryStream.Position = 0;

                        // 1. Memorizza il file Excel in MongoDB
                        var mongoClient = new MongoClient("mongodb://localhost:27017");
                        var mongoDatabase = mongoClient.GetDatabase("UniBiblio");
                        var gridFSBucket = new GridFSBucket(mongoDatabase);

                        var fileId = await gridFSBucket.UploadFromStreamAsync(
                            "StatistichePrenotazioniMensili.xlsx", memoryStream,
                            new GridFSUploadOptions
                            {
                                Metadata = new BsonDocument
                                {
                            { "createdBy", "admin@unibiblio.com" },
                            { "reportType", "MonthlyAnalytics" },
                            { "createdAt", DateTime.UtcNow }
                                }
                            }
                        );

                        // 2. Restituisce il file Excel come download all'utente
                        memoryStream.Position = 0;
                        var fileName = "StatistichePrenotazioniMensili.xlsx";
                        return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                // Gestisci eventuali errori
                Console.WriteLine($"Errore: {ex.Message}");
                return StatusCode(500, "Errore durante la generazione del file Excel");
            }
        }

        [HttpPost]
        public IActionResult CancellaPrenotazioneLibro(int idPrenotazione)
        {
            var reservation = _context.PrenotazioniLibris.SingleOrDefault(r => (int)r.IdPrenotazione == idPrenotazione);

            if (reservation != null)
            {
                reservation.Stato = "Cancellato";  // Set the status to "Cancellato"
                _context.SaveChanges();  // Save the changes to the database
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CancellaPrenotazioneSala(int idPrenotazione)
        {
            var reservation = _context.PrenotazioniSales.SingleOrDefault(r => (int)r.IdPrenotazione == idPrenotazione);

            if (reservation != null)
            {
                reservation.Stato = "Cancellato";  // Set the status to "Cancellato"
                _context.SaveChanges();  // Save the changes to the database
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ConfermaRitiro(int idPrenotazione)
        {
            var reservation = _context.PrenotazioniLibris.SingleOrDefault(r => (int)r.IdPrenotazione == idPrenotazione);

            if (reservation != null && reservation.Stato == "Prenotato")
            {
                reservation.DataRitiro = DateOnly.FromDateTime(DateTime.Now);  // Set the current date as the ritiro date
                reservation.Stato = "Ritirato";  // Update the status to "Ritirato"
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ConfermaRestituzione(int idPrenotazione)
        {
            var reservation = _context.PrenotazioniLibris.SingleOrDefault(r => (int)r.IdPrenotazione == idPrenotazione);

            if (reservation != null && reservation.Stato == "Ritirato")
            {
                reservation.Stato = "Restituito";  // Update the status to "Restituito"
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }

}
