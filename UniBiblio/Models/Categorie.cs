using System;
using System.Collections.Generic;

namespace UniBiblio.Models
{
    public partial class Categorie
    {
        public ulong IdCategoria { get; set; }
        public string NomeCategoria { get; set; } = null!;
    }
}
