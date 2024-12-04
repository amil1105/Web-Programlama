using Microsoft.AspNetCore.Mvc;
using WebProje.Models;

namespace WebProje.Controllers
{
    public class RegisterController : BaseController
    {
        private readonly KuaforContext _context;

        public RegisterController(KuaforContext context) : base(context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Kullanici kullanici)
        {
            if (ModelState.IsValid)
            {
                // E-posta kontrolü
                var mevcutKullanici = _context.Kullanicilar.FirstOrDefault(k => k.Email == kullanici.Email);
                if (mevcutKullanici != null)
                {
                    ViewBag.ErrorMessage = "Bu e-posta zaten kayıtlı.";
                    return View(kullanici);
                }

                // Yeni kullanıcıyı veritabanına ekle
                _context.Kullanicilar.Add(kullanici);
                _context.SaveChanges();

                // Kayıt sonrası giriş sayfasına yönlendirme
                return RedirectToAction("Index", "Login");
            }

            return View(kullanici);
        }
    }
}
