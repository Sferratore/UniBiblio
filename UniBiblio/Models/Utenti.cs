using System;
using System.Collections.Generic;

namespace UniBiblio.Models
{
    public partial class Utenti
    {
        public ulong IdUtente { get; set; }
        public string Nome { get; set; } = null!;
        public string Cognome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? Telefono { get; set; }
        public int? IdRuolo { get; set; }
    }
}
