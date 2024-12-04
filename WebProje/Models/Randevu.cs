using System;
using System.ComponentModel.DataAnnotations;

namespace WebProje.Models
{
    public class Randevu
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Çalışan seçimi zorunludur.")]
        public int CalisanId { get; set; }
        public Calisan Calisan { get; set; }

        [Required(ErrorMessage = "İşlem seçimi zorunludur.")]
        public int IslemId { get; set; }
        public Islem Islem { get; set; }

        [Required(ErrorMessage = "Tarih seçimi zorunludur.")]
        [DataType(DataType.Date)]
        public DateTime Tarih { get; set; }

        [Required(ErrorMessage = "Saat seçimi zorunludur.")]
        [DataType(DataType.Time)]
        public TimeSpan Saat { get; set; }

        [Required(ErrorMessage = "Kullanıcı ID zorunludur.")]
        public string KullaniciId { get; set; }
    }
}
