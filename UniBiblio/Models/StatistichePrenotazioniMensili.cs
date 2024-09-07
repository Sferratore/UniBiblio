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
        public int? PrenotazioniLibriPrenotati { get; set; }
        public int? PrenotazioniLibriRitirati { get; set; }
        public int? PrenotazioniLibriRestituiti { get; set; }
        public int? PrenotazioniLibriCancellati { get; set; }
        public int? PrenotazioniSalePrenotate { get; set; }
        public int? PrenotazioniSaleUsufruite { get; set; }
        public int? PrenotazioniSaleCancellate { get; set; }
        public string? LibroPiuPrenotato { get; set; }
        public string? SalaPiuPrenotata { get; set; }
        public DateTime? CreatoIl { get; set; }
    }
}
