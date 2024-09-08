using System;
using System.Collections.Generic;

namespace UniBiblio.Models
{
    public partial class PrenotazioniLibri
    {
        public ulong IdPrenotazione { get; set; }
        public ulong? IdUtente { get; set; }
        public ulong? IdLibro { get; set; }
        public DateOnly DataPrenotazione { get; set; }
        public DateOnly? DataRitiro { get; set; }
        public DateOnly? DataRestituzione { get; set; }
        public string Stato { get; set; } = null!;

        public virtual Libri? IdLibroNavigation { get; set; }
        public virtual Utenti? IdUtenteNavigation { get; set; }
    }
}
