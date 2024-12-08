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
            return View();
        }
        [HttpPost]
        public IActionResult RandevuOlustur(Randevu randevu)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                ViewBag.Calisanlar = _context.Calisanlar.ToList();
                ViewBag.Islemler = _context.Islemler.ToList();
                ViewBag.ErrorMessage = "Oturumunuz sona ermiş. Lütfen tekrar giriş yapınız.";
                return View(randevu);
            }

            randevu.KullaniciId = userId;

          

            // Çakışan randevu kontrolü
            bool isConflict = _context.Randevular.Any(r =>
                r.CalisanId == randevu.CalisanId &&
                r.Tarih == randevu.Tarih &&
                r.Saat == randevu.Saat);

            if (isConflict)
            {
                ViewBag.Calisanlar = _context.Calisanlar.ToList();
                ViewBag.Islemler = _context.Islemler.ToList();
                ViewBag.ErrorMessage = "Bu saat dolu, lütfen başka bir saat seçiniz.";
                return View(randevu);
            }

            try
            {
                _context.Randevular.Add(randevu);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Randevunuz başarıyla oluşturuldu!";
                return RedirectToAction("Randevularim");
            }
            catch (Exception ex)
            {
                ViewBag.Calisanlar = _context.Calisanlar.ToList();
                ViewBag.Islemler = _context.Islemler.ToList();
                ViewBag.ErrorMessage = $"Veritabanına kaydetme sırasında hata oluştu: {ex.Message}";
                return View(randevu);
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
