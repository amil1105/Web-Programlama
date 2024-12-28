using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProje.Models;

namespace WebProje.Controllers
{
    public class RandevuController : BaseController
    {
        public RandevuController(KuaforContext context) : base(context) { }

        [HttpGet]
        public IActionResult RandevuOlustur()
        {
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Index", "Login");
            }

            if (!long.TryParse(userIdString, out var userId))
            {
                TempData["ErrorMessage"] = "Geçersiz kullanıcı bilgisi!";
                return RedirectToAction("Index", "Login");
            }

            ViewBag.Calisanlar = _context.Calisanlar.ToList();
            ViewBag.Islemler = _context.Islemler.ToList();

            var magaza = _context.Magazalar.FirstOrDefault();
            ViewBag.AcilisSaati = magaza?.AcilisSaati.ToString(@"hh\:mm");
            ViewBag.KapanisSaati = magaza?.KapanisSaati.ToString(@"hh\:mm");

            return View(new Randevu());
        }

        [HttpPost]
        public IActionResult RandevuOlustur(Randevu randevu)
        {
            var kullaniciId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(kullaniciId))
            {
                TempData["ErrorMessage"] = "Oturum sona ermiş. Lütfen tekrar giriş yapınız.";
                return RedirectToAction("Index", "Login");
            }

            if (!long.TryParse(kullaniciId, out var kullaniciIdLong))
            {
                TempData["ErrorMessage"] = "Geçersiz kullanıcı bilgisi!";
                return RedirectToAction("Index", "Login");
            }

            randevu.KullaniciId = kullaniciIdLong.ToString();

            // Mağaza saatlerini kontrol et
            var magaza = _context.Magazalar.FirstOrDefault();
            if (magaza == null)
            {
                TempData["ErrorMessage"] = "Mağaza bilgileri bulunamadı.";
                return RedirectToAction("RandevuOlustur");
            }

            var calisan = _context.Calisanlar
                .Include(c => c.CalisanIslemler)
                .FirstOrDefault(c => c.Id == randevu.CalisanId);

            if (calisan == null)
            {
                TempData["ErrorMessage"] = "Seçilen çalışan bulunamadı.";
                return RedirectToAction("RandevuOlustur");
            }
            //Pazartesi,Sali,Carsamba,Persembe,Cuma,Cumartesi
            // Günlerin Türkçe karşılığını eşleştir
            Dictionary<DayOfWeek, string> gunler = new Dictionary<DayOfWeek, string>
    {
        { DayOfWeek.Monday, "Pazartesi" },
        { DayOfWeek.Tuesday, "Sali" },
        { DayOfWeek.Wednesday, "Carsamba" },
        { DayOfWeek.Thursday, "Persembe" },
        { DayOfWeek.Friday, "Cuma" },
        { DayOfWeek.Saturday, "Cumartesi" },
        { DayOfWeek.Sunday, "Pazar" }
    };

            var randevuGun = gunler[randevu.Tarih.DayOfWeek];

            // Çalışanın çalıştığı gün kontrolü
            if (!calisan.CalismaGunleriListesi.Any(g => g.ToString() == randevuGun))
            {
                TempData["ErrorMessage"] = "Seçilen çalışan bu gün çalışmıyor.";
                return RedirectToAction("RandevuOlustur");
            }

            // Randevu saati kontrolü
            var randevuBaslangic = randevu.Tarih + randevu.Saat;
            var randevuBitis = randevuBaslangic.AddMinutes(_context.Islemler.FirstOrDefault(i => i.Id == randevu.IslemId)?.Sure ?? 0);

            if (randevu.Saat < calisan.CalismaBaslangicSaati || randevuBitis.TimeOfDay > calisan.CalismaBitisSaati)
            {
                TempData["ErrorMessage"] = "Seçilen çalışan bu saatlerde çalışmıyor.";
                return RedirectToAction("RandevuOlustur");
            }

            if (randevu.Saat < magaza.AcilisSaati || randevuBitis.TimeOfDay > magaza.KapanisSaati)
            {
                TempData["ErrorMessage"] = "Randevu saati mağaza çalışma saatleri dışında.";
                return RedirectToAction("RandevuOlustur");
            }

            // Çalışanın müsaitlik kontrolü
            var mevcutRandevular = _context.Randevular
                .Where(r => r.CalisanId == randevu.CalisanId && r.Tarih == randevu.Tarih)
                .ToList();

            bool calisanMesgulMu = mevcutRandevular.Any(r =>
            {
                var mevcutIslemSuresi = _context.Islemler.FirstOrDefault(i => i.Id == r.IslemId)?.Sure ?? 0;
                var mevcutBaslangic = r.Tarih + r.Saat;
                var mevcutBitis = mevcutBaslangic.AddMinutes(mevcutIslemSuresi);

                return (randevuBaslangic >= mevcutBaslangic && randevuBaslangic < mevcutBitis) ||
                       (randevuBitis > mevcutBaslangic && randevuBitis <= mevcutBitis);
            });

            if (calisanMesgulMu)
            {
                TempData["ErrorMessage"] = "Seçilen çalışan bu saatte meşgul.";
                return RedirectToAction("RandevuOlustur");
            }

            _context.Randevular.Add(randevu);

            try
            {
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Randevu başarıyla oluşturuldu.";
                return RedirectToAction("Randevularim", "Randevu");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Randevu kaydedilirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("RandevuOlustur");
            }
        }



        [HttpGet]
        public IActionResult GetCalisanDetails(int id)
        {
            var calisan = _context.Calisanlar.FirstOrDefault(c => c.Id == id);
            if (calisan != null)
            {
                return Json(new
                {
                    adSoyad = $"{calisan.Ad} {calisan.Soyad}",
                    telefon = calisan.Telefon,
                  //  uzmanlikAlanlari = calisan.UzmanlikAlanlari,
                    adres = calisan.Adres,
                    profilFotoPath = string.IsNullOrEmpty(calisan.ProfilFotoPath)
                        ? "/uploads/default-profile.png"
                        : Url.Content(calisan.ProfilFotoPath)
                });
            }
            return Json(null);
        }

        [HttpGet]
        public IActionResult GetCalisanIslemler(int calisanId)
        {
            // Çalışanın yapabildiği işlemleri getir
            var islemIds = _context.CalisanIslemler
                .Where(ci => ci.CalisanId == calisanId)
                .Select(ci => ci.IslemId)
                .ToList();

            var islemler = _context.Islemler
                .Where(i => islemIds.Contains(i.Id))
                .Select(i => new { i.Id, i.Ad, i.Ucret, i.Sure })
                .ToList();

            return Json(islemler);
        }



        [HttpGet]
        public IActionResult Randevularim()
        {
            // Kullanıcı oturum kontrolü
            var kullaniciId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(kullaniciId))
            {
                TempData["ErrorMessage"] = "Oturum süresi dolmuş. Lütfen tekrar giriş yapınız.";
                return RedirectToAction("Index", "Login");
            }

            // Kullanıcıya ait randevuları çek
            var randevular = _context.Randevular
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .Where(r => r.KullaniciId == kullaniciId) // Kullanıcıya ait randevular
                .OrderBy(r => r.Tarih)
                .ThenBy(r => r.Saat)
                .ToList();

            return View(randevular);
        }


        [HttpGet]
        public IActionResult TumRandevular()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişim yetkiniz yok.";
                return RedirectToAction("Index", "Login");
            }

            var randevular = _context.Randevular
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .AsEnumerable() 
                .Select(r => new RandevuViewModel
                {
                    Id = r.Id,
                    CalisanAdSoyad = r.Calisan.Ad + " " + r.Calisan.Soyad,
                    IslemAd = r.Islem.Ad,
                    Tarih = r.Tarih,
                    Saat = r.Saat,
                    KullaniciId = r.KullaniciId,
                    KullaniciAdSoyad = _context.Kullanicilar
                        .FirstOrDefault(k => k.Id.ToString() == r.KullaniciId)?.Ad + " " +
                                          _context.Kullanicilar
                        .FirstOrDefault(k => k.Id.ToString() == r.KullaniciId)?.Soyad,
                    KullaniciTelefon = _context.Kullanicilar
                        .FirstOrDefault(k => k.Id.ToString() == r.KullaniciId)?.Telefon
                })
                .ToList();

            return View(randevular);
        }




    }
}
