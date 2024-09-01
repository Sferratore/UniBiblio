using System;
using System.Collections.Generic;

namespace UniBiblio.Models
{
    public partial class Prenotasaleview
    {
        public ulong IdSala { get; set; }
        public string NomeSala { get; set; } = null!;
        public string Biblioteca { get; set; } = null!;
        public int Capienza { get; set; }
        public bool? Disponibilita { get; set; }
        public string IndirizzoBiblioteca { get; set; } = null!;
        public long PostiDisponibili { get; set; }
    }
}
