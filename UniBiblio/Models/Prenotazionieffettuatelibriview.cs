using System;
using System.Collections.Generic;

namespace UniBiblio.Models
{
    public partial class Prenotazionieffettuatelibriview
    {
        public ulong IdPrenotazione { get; set; }
        public ulong? IdUtente { get; set; }
        public ulong? IdLibro { get; set; }
        public string EmailUtente { get; set; } = null!;
        public string TitoloLibro { get; set; } = null!;
        public string Isbn { get; set; } = null!;
        public string NomeBiblioteca { get; set; } = null!;
        public DateOnly DataPrenotazione { get; set; }
        public DateOnly? DataRitiro { get; set; }
        public string Stato { get; set; } = null!;
    }
}
