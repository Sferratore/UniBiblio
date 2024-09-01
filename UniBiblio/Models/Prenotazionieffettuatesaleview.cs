using System;
using System.Collections.Generic;

namespace UniBiblio.Models
{
    public partial class Prenotazionieffettuatesaleview
    {
        public ulong IdPrenotazione { get; set; }
        public int? IdUtente { get; set; }
        public string EmailUtente { get; set; } = null!;
        public string NomeSala { get; set; } = null!;
        public DateOnly DataPrenotazione { get; set; }
        public TimeOnly OraInizio { get; set; }
        public TimeOnly OraFine { get; set; }
        public string Stato { get; set; } = null!;
        public string Biblioteca { get; set; } = null!;
    }
}
