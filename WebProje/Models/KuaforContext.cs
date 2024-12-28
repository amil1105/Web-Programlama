using Microsoft.EntityFrameworkCore;

namespace WebProje.Models
{
    public class KuaforContext : DbContext
    {
        public KuaforContext(DbContextOptions<KuaforContext> options) : base(options) { }

        public DbSet<Kullanici> Kullanicilar { get; set; } // Kullanıcılar için DbSet
        public DbSet<Calisan> Calisanlar { get; set; }
        public DbSet<Magaza> Magazalar { get; set; }
        public DbSet<Islem> Islemler { get; set; }
        public DbSet<Randevu> Randevular { get; set; }

       public DbSet<CalisanIslem> CalisanIslemler { get; set; }
        // Diğer DbSet'ler
        public DbSet<HomepageCard> HomepageCards { get; set; } // Bu satırı ekle

    }


}
