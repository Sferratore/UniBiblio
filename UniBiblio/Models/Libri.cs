using System;
using System.Collections.Generic;

namespace UniBiblio.Models
{
    public partial class Libri
    {
        public Libri()
        {
            PrenotazioniLibris = new HashSet<PrenotazioniLibri>();
            IdCategoria = new HashSet<Categorie>();
        }

        public ulong IdLibro { get; set; }
        public string Titolo { get; set; } = null!;
        public string Autore { get; set; } = null!;
        public int? AnnoPubblicazione { get; set; }
        public string Isbn { get; set; } = null!;
        public ulong? IdBiblioteca { get; set; }
        public int? QuantitaDisponibile { get; set; }
        public string? Categoria { get; set; }

        public virtual Biblioteche? IdBibliotecaNavigation { get; set; }
        public virtual ICollection<PrenotazioniLibri> PrenotazioniLibris { get; set; }

        public virtual ICollection<Categorie> IdCategoria { get; set; }
    }
}
