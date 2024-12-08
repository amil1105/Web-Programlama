namespace WebProje.Models
{
    public class Magaza
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Adres { get; set; }

        public Int64 Telefon { get; set; }

        public TimeSpan AcilisSaati { get; set; } 
        public TimeSpan KapanisSaati { get; set; } 
    }
}
