﻿using System;
using System.Collections.Generic;

namespace UniBiblio.Models
{
    public partial class PrenotazioniSale
    {
        public ulong IdPrenotazione { get; set; }
        public int? IdUtente { get; set; }
        public int? IdSala { get; set; }
        public DateOnly DataPrenotazione { get; set; }
        public TimeOnly OraInizio { get; set; }
        public TimeOnly OraFine { get; set; }
        public string Stato { get; set; } = null!;
    }
}
