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
    }
}
