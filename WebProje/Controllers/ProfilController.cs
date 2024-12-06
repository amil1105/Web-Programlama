using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebProje.Models;

namespace WebProje.Controllers
{
    public class ProfilController : BaseController
    {
        private readonly KuaforContext _context;

        public ProfilController(KuaforContext context) : base(context)
        {
            _context = context;
        }
       
        [HttpGet]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewData["UserRole"] = userRole;

            var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.Id == int.Parse(userId));
            return View(kullanici);
        }

        [HttpPost]
        public IActionResult Index(Kullanici guncelKullanici)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetString("UserId");

                if (userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                // Mevcut kullanıcı bilgilerini güncelle
                var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.Id == int.Parse(userId));
                if (kullanici != null)
                {
                    kullanici.Ad = guncelKullanici.Ad;
                    kullanici.Soyad = guncelKullanici.Soyad;
                    kullanici.Email = guncelKullanici.Email;
                    kullanici.Telefon = guncelKullanici.Telefon;
                    _context.SaveChanges();
                }

                ViewBag.Message = "Profil başarıyla güncellendi!";
            }

            return View(guncelKullanici);
        }

        // Şifre değiştirme
        [HttpPost]
        public IActionResult SifreDegistir(string eskiSifre, string yeniSifre)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.Id == int.Parse(userId));
            if (kullanici != null && kullanici.Sifre == eskiSifre)
            {
                kullanici.Sifre = yeniSifre;
                _context.SaveChanges();
                ViewBag.Message = "Şifreniz başarıyla değiştirildi!";
            }
            else
            {
                ViewBag.ErrorMessage = "Eski şifre yanlış!";
            }

            return RedirectToAction("Index");
        }
    }
}
