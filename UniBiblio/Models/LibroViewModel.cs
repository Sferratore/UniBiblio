namespace UniBiblio.Models
{
    public class LibroViewModel
    {
        public int Id { get; set; }
        public string Titolo { get; set; }
        public string Autore { get; set; }
        public int? AnnoPubblicazione { get; set; }
        public string ISBN { get; set; }
        public string Categoria { get; set; }
        public int QuantitaDisponibile { get; set; }
        public bool IsPrenotabile => QuantitaDisponibile > 0;
    }
}
