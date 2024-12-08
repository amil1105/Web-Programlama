using System.Collections.Generic;

namespace WebProje.Models
{
    public class RaporViewModel
    {
        public List<CalisanRaporu> CalisanBazindaIslem { get; set; }
        public List<PopulerIslemRaporu> PopulerIslemler { get; set; }
        public decimal MagazaGunlukKazanc { get; set; }

        public string SeciliCalisan { get; set; }
        public DateTime BaslangicTarihi { get; internal set; }
        public DateTime BitisTarihi { get; internal set; }
        public List<Calisan> Calisanlar { get; internal set; }

    }

    public class CalisanRaporu
    {
        public string CalisanAdSoyad { get; set; }
        public int IslemSayisi { get; set; }
        public decimal ToplamKazanc { get; set; }
        public string CalisanAdi { get; internal set; }
    }

    public class PopulerIslemRaporu
    {
        public string IslemAdi { get; set; }
        public int IslemSayisi { get; set; }
    }

}
