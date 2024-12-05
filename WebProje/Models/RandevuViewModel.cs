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
    }
}
