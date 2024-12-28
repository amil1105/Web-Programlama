namespace WebProje.Models
{
    public class Calisan
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
      

        public Int64 Telefon {  get; set; }
        
        public string Adres {  get; set; }

        public string? ProfilFotoPath { get; set; }


        // Çalışanın yapabildiği işlemler
        public List<CalisanIslem> CalisanIslemler { get; set; } = new List<CalisanIslem>();

        public string CalismaGunleri { get; set; }
        public TimeSpan CalismaBaslangicSaati { get; set; }
        public TimeSpan CalismaBitisSaati { get; set; }
        public List<Gunler> CalismaGunleriListesi
        {
            get
            {
                return string.IsNullOrEmpty(CalismaGunleri)
                    ? new List<Gunler>()
                    : CalismaGunleri.Split(',').Select(g => Enum.Parse<Gunler>(g)).ToList();
            }
        }
    }
}
