namespace WebProje.Models
{
    public class Magaza
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Adres { get; set; }
        public string CalismaSaatleri { get; set; }
        public ICollection<Calisan> Calisanlar { get; set; }
    }
}
