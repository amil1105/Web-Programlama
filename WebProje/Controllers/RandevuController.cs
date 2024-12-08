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
            var userId = HttpContext.Session.GetString("UserId");

            if (userId == null)
            {
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

            randevu.KullaniciId = kullaniciId;

            var magaza = _context.Magazalar.FirstOrDefault();
            if (magaza == null)
            {
                TempData["ErrorMessage"] = "Mağaza bilgileri bulunamadı.";
                return RedirectToAction("RandevuOlustur");
            }

            if (randevu.Tarih < DateTime.Today || randevu.Tarih > DateTime.Today.AddYears(1))
            {
                TempData["ErrorMessage"] = "Geçerli bir tarih seçiniz.";
                return RedirectToAction("RandevuOlustur");
            }

            if (randevu.Saat < magaza.AcilisSaati || randevu.Saat > magaza.KapanisSaati)
            {
                TempData["ErrorMessage"] = $"Saat mağaza çalışma saatleri arasında olmalıdır ({magaza.AcilisSaati} - {magaza.KapanisSaati}).";
                return RedirectToAction("RandevuOlustur");
            }

            var islemSuresi = _context.Islemler.FirstOrDefault(i => i.Id == randevu.IslemId)?.Sure ?? 0;
            var randevuBaslangic = randevu.Tarih.Add(randevu.Saat);
            var randevuBitis = randevuBaslangic.AddMinutes(islemSuresi);

            // Mevcut randevuları veri tabanından çek
            var mevcutRandevular = _context.Randevular
                .Where(r => r.CalisanId == randevu.CalisanId && r.Tarih == randevu.Tarih)
                .ToList(); // Belleğe al

            // Çalışanın uygunluk durumunu kontrol et
            var calisanMesgulMu = mevcutRandevular.Any(r =>
            {
                var mevcutBaslangic = r.Tarih.Add(r.Saat);
                var mevcutBitis = mevcutBaslangic.AddMinutes(_context.Islemler.FirstOrDefault(i => i.Id == r.IslemId)?.Sure ?? 0);

                return (randevuBaslangic >= mevcutBaslangic && randevuBaslangic < mevcutBitis) || // Yeni randevu mevcut randevunun içinde mi?
                       (randevuBitis > mevcutBaslangic && randevuBitis <= mevcutBitis) ||         // Yeni randevu mevcut randevunun sonunda mı?
                       (randevuBaslangic <= mevcutBaslangic && randevuBitis >= mevcutBitis);     // Yeni randevu mevcut randevuyu kapsıyor mu?
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
                return RedirectToAction("Index", "Home");
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
            if (calisan == null)
            {
                return NotFound();
            }

            var calisanDetails = new
            {
                adSoyad = $"{calisan.Ad} {calisan.Soyad}",
                telefon = calisan.Telefon.ToString(),
                uzmanlikAlanlari = calisan.UzmanlikAlanlari ?? "Belirtilmemiş",
                adres = calisan.Adres ?? "Belirtilmemiş",
                profilFotoPath = calisan.ProfilFotoPath ?? "/uploads/default-profile.png"
            };

            return Json(calisanDetails);
        }


        public IActionResult Randevularim()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Lütfen giriş yapınız.";
                return RedirectToAction("Index", "Login");
            }

            var randevular = _context.Randevular
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .Where(r => r.KullaniciId == userId)
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
