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
                var mevcutKullanici = _context.Kullanicilar.FirstOrDefault(k => k.Email == kullanici.Email);
                if (mevcutKullanici != null)
                {
                    ViewBag.ErrorMessage = "Bu e-posta zaten kayıtlı.";
                    return View(kullanici);
                }

                var mevcutKullanici2 = _context.Kullanicilar.FirstOrDefault(k => k.Telefon == kullanici.Telefon);
                if (mevcutKullanici != null)
                {
                    ViewBag.ErrorMessage = "Bu Telefon No zaten kayıtlı.";
                    return View(kullanici);
                }

                _context.Kullanicilar.Add(kullanici);
                _context.SaveChanges();

                return RedirectToAction("Index", "Login");
            }

            return View(kullanici);
        }
    }
}
