using System;
using System.Collections.Generic;

namespace UniBiblio.Models
{
    public partial class SaleStudio
    {
        public ulong IdSala { get; set; }
        public string NomeSala { get; set; } = null!;
        public int? IdBiblioteca { get; set; }
        public int Capienza { get; set; }
        public bool? Disponibilita { get; set; }
    }
}
