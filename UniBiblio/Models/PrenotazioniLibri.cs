namespace UniBiblio.Models
{
    public partial class PrenotazioniLibri
    {
        public ulong IdPrenotazione { get; set; }
        public int? IdUtente { get; set; }
        public int? IdLibro { get; set; }
        public DateOnly DataPrenotazione { get; set; }
        public DateOnly? DataRitiro { get; set; }
        public DateOnly? DataRestituzione { get; set; }
        public string Stato { get; set; } = null!;
    }
}
