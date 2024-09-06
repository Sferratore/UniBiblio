using Dapper;
using Microsoft.AspNetCore.Mvc;
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
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Analytics()
        {

            try
            {
                //Qui usiamo Dapper perché EFC non ci permette di ottenere risultati dal DB se non si tratta di tabelle o view che comunicano col dbcontext
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var sale = await connection.ExecuteAsync(
                        "CALL InsertMonthlyStatistics()"
                    );

                    // Recupera i dati dalla tabella Statistiche_Prenotazioni_Mensili
                    var statistiche = await connection.QueryAsync<StatistichePrenotazioniMensili>(
                        "SELECT * FROM Statistiche_Prenotazioni_Mensili"
                    );

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
                            worksheet.Cells[1, 5].Value = "Prenotazioni Libri Completate";
                            worksheet.Cells[1, 6].Value = "Prenotazioni Libri Attive";
                            worksheet.Cells[1, 7].Value = "Prenotazioni Sale Confermate";
                            worksheet.Cells[1, 8].Value = "Libro Più Prenotato";
                            worksheet.Cells[1, 9].Value = "Sala Più Prenotata";

                            // Aggiungi i dati delle statistiche
                            int row = 2; // La prima riga è per le intestazioni
                            foreach (var statistica in statistiche)
                            {
                                worksheet.Cells[row, 1].Value = statistica.Mese;
                                worksheet.Cells[row, 2].Value = statistica.Anno;
                                worksheet.Cells[row, 3].Value = statistica.TotalePrenotazioniLibri;
                                worksheet.Cells[row, 4].Value = statistica.TotalePrenotazioniSale;
                                worksheet.Cells[row, 5].Value = statistica.PrenotazioniLibriCompletate;
                                worksheet.Cells[row, 6].Value = statistica.PrenotazioniLibriAttive;
                                worksheet.Cells[row, 7].Value = statistica.PrenotazioniSaleConfermate;
                                worksheet.Cells[row, 8].Value = statistica.LibroPiuPrenotato;
                                worksheet.Cells[row, 9].Value = statistica.SalaPiuPrenotata;
                                row++;
                            }

                            // Formatta il foglio di lavoro (facoltativo)
                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                            worksheet.Cells[1, 1, 1, 9].Style.Font.Bold = true; // Rendi in grassetto l'intestazione

                            // Salva il pacchetto Excel nel memory stream
                            package.Save();
                        }

                        // Restituisce il file Excel come download
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
    }
}
