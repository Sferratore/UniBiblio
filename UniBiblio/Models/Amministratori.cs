using System;
using System.Collections.Generic;

namespace UniBiblio.Models
{
    public partial class Amministratori
    {
        public ulong IdAdmin { get; set; }
        public string Nome { get; set; } = null!;
        public string Cognome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public int? IdBiblioteca { get; set; }
    }
}
