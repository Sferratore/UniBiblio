using System;
using System.Collections.Generic;

namespace UniBiblio.Models
{
    public partial class SaleStudio
    {
        public SaleStudio()
        {
            PrenotazioniSales = new HashSet<PrenotazioniSale>();
        }

        public ulong IdSala { get; set; }
        public string NomeSala { get; set; } = null!;
        public ulong? IdBiblioteca { get; set; }
        public int Capienza { get; set; }
        public bool? Disponibilita { get; set; }

        public virtual Biblioteche? IdBibliotecaNavigation { get; set; }
        public virtual ICollection<PrenotazioniSale> PrenotazioniSales { get; set; }
    }
}
