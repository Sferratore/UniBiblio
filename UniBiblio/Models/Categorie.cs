using System;
using System.Collections.Generic;

namespace UniBiblio.Models
{
    public partial class Categorie
    {
        public Categorie()
        {
            IdLibros = new HashSet<Libri>();
        }

        public ulong IdCategoria { get; set; }
        public string NomeCategoria { get; set; } = null!;

        public virtual ICollection<Libri> IdLibros { get; set; }
    }
}
