using System;
using System.Collections.Generic;

namespace UniBiblio.Models
{
    public partial class Libri
    {
        public ulong IdLibro { get; set; }
        public string Titolo { get; set; } = null!;
        public string Autore { get; set; } = null!;
        public int? AnnoPubblicazione { get; set; }
        public string Isbn { get; set; } = null!;
        public int? IdBiblioteca { get; set; }
        public int? QuantitaDisponibile { get; set; }
        public string? Categoria { get; set; }
    }
}
