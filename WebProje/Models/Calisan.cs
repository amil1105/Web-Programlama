namespace WebProje.Models
{
    public class Calisan
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string UzmanlikAlanlari { get; set; } // Örnek: "Saç Kesimi, Sakal Kesimi"
        public string UygunlukSaatleri { get; set; } // Örnek: "09:00-18:00"
        public int MagazaId { get; set; }
        public Magaza Magaza { get; set; }
        public ICollection<Randevu> Randevular { get; set; }
    }
}
