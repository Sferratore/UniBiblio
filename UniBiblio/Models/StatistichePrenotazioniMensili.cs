using System;
using System.Collections.Generic;

namespace UniBiblio.Models
{
    public partial class StatistichePrenotazioniMensili
    {
        public ulong IdStatistica { get; set; }
        public int? Mese { get; set; }
        public int? Anno { get; set; }
        public int? TotalePrenotazioniLibri { get; set; }
        public int? TotalePrenotazioniSale { get; set; }
        public int? PrenotazioniLibriCompletate { get; set; }
        public int? PrenotazioniLibriAttive { get; set; }
        public int? PrenotazioniSaleConfermate { get; set; }
        public string? LibroPiuPrenotato { get; set; }
        public string? SalaPiuPrenotata { get; set; }
        public DateTime? CreatoIl { get; set; }
    }
}
