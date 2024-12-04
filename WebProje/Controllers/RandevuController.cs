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
            ViewBag.Calisanlar = _context.Calisanlar.ToList();
            ViewBag.Islemler = _context.Islemler.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult RandevuOlustur(Randevu randevu)
        {
            // Kullanıcı oturumundan UserId'yi al
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                ViewBag.Calisanlar = _context.Calisanlar.ToList();
                ViewBag.Islemler = _context.Islemler.ToList();
                ViewBag.ErrorMessage = "Oturumunuz sona ermiş. Lütfen tekrar giriş yapınız.";
                return View(randevu);
            }

            // Kullanıcı ID'sini modele ekle
            randevu.KullaniciId = userId;

            // Model doğrulama kontrolü
            if (!ModelState.IsValid)
            {
                ViewBag.Calisanlar = _context.Calisanlar.ToList();
                ViewBag.Islemler = _context.Islemler.ToList();

                // Hata mesajlarını topla
                ViewBag.ErrorMessage = string.Join("<br>", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                return View(randevu);
            }

            try
            {
                // Randevuyu kaydet
                _context.Randevular.Add(randevu);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Randevunuz başarıyla oluşturuldu!";
                return RedirectToAction("Randevularim");
            }
            catch (Exception ex)
            {
                ViewBag.Calisanlar = _context.Calisanlar.ToList();
                ViewBag.Islemler = _context.Islemler.ToList();
                ViewBag.ErrorMessage = "Randevu kaydedilirken bir hata oluştu: " + ex.Message;
                return View(randevu);
            }
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
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişim yetkiniz yok.";
                return RedirectToAction("Index", "Login");
            }

            var randevular = _context.Randevular
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .OrderBy(r => r.Tarih)
                .ThenBy(r => r.Saat)
                .ToList();

            return View(randevular);
        }
    }
}
