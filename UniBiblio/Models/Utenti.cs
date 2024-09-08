using System;
using System.Collections.Generic;

namespace UniBiblio.Models
{
    public partial class Utenti
    {
        public Utenti()
        {
            PrenotazioniLibris = new HashSet<PrenotazioniLibri>();
            PrenotazioniSales = new HashSet<PrenotazioniSale>();
        }

        public ulong IdUtente { get; set; }
        public string Nome { get; set; } = null!;
        public string Cognome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? Telefono { get; set; }
        public bool? IsAmministratore { get; set; }

        public virtual ICollection<PrenotazioniLibri> PrenotazioniLibris { get; set; }
        public virtual ICollection<PrenotazioniSale> PrenotazioniSales { get; set; }
    }
}
