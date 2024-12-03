namespace WebProje.Models
{
    public class Randevu
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public Kullanici Kullanici { get; set; }
        public int CalisanId { get; set; }
        public Calisan Calisan { get; set; }
        public int IslemId { get; set; }
        public Islem Islem { get; set; }
        public DateTime Tarih { get; set; }
        public string Saat { get; set; }
        public bool OnaylandiMi { get; set; }
    }
}
