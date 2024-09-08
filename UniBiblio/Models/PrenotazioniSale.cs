using System;
using System.Collections.Generic;

namespace UniBiblio.Models
{
    public partial class PrenotazioniSale
    {
        public ulong IdPrenotazione { get; set; }
        public ulong? IdUtente { get; set; }
        public ulong? IdSala { get; set; }
        public DateOnly DataPrenotazione { get; set; }
        public DateOnly GiornoPrenotato { get; set; }
        public TimeOnly OraInizio { get; set; }
        public TimeOnly OraFine { get; set; }
        public string Stato { get; set; } = null!;

        public virtual SaleStudio? IdSalaNavigation { get; set; }
        public virtual Utenti? IdUtenteNavigation { get; set; }
    }
}
