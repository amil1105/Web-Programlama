namespace WebProje.Models
{
    public class RandevuViewModel
    {
        public int Id { get; set; }
        public string CalisanAdSoyad { get; set; }
        public string IslemAd { get; set; }
        public DateTime Tarih { get; set; }
        public TimeSpan Saat { get; set; }
        public string KullaniciId { get; set; }
        public string KullaniciAdSoyad { get; set; }
        public string KullaniciTelefon { get; set; }

        public List<Calisan> Calisanlar { get; set; }
        public List<Islem> Islemler { get; set; }
        public DateTime Bugun { get; set; }
        public Randevu Randevu { get; set; }

        public TimeSpan AcilisSaati { get; internal set; }

        public TimeSpan KapanisSaati { get; set; } 
    }
}
