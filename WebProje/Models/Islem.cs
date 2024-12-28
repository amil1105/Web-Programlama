using Microsoft.EntityFrameworkCore;

namespace WebProje.Models
{
    public class Islem
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public int Sure { get; set; } // Dakika cinsinden süre
        [Precision(18, 2)]
        public decimal Ucret { get; set; }

        public List<CalisanIslem> CalisanIslemler { get; set; } = new List<CalisanIslem>();
    }
}
