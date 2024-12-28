namespace WebProje.Models
{
    public class CalisanGun
    {
        public int Id { get; set; }
        public int CalisanId { get; set; }
        public Calisan Calisan { get; set; }
        public Gunler Gun { get; set; } // Enum türü
    }
}
