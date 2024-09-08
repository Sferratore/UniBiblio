using System;
using System.Collections.Generic;

namespace UniBiblio.Models
{
    public partial class Biblioteche
    {
        public Biblioteche()
        {
            Libris = new HashSet<Libri>();
            SaleStudios = new HashSet<SaleStudio>();
        }

        public ulong IdBiblioteca { get; set; }
        public string NomeBiblioteca { get; set; } = null!;
        public string Indirizzo { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? OrariApertura { get; set; }

        public virtual ICollection<Libri> Libris { get; set; }
        public virtual ICollection<SaleStudio> SaleStudios { get; set; }
    }
}
