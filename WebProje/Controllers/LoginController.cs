using Microsoft.AspNetCore.Mvc;
using WebProje.Models;

namespace WebProje.Controllers
{
    public class LoginController : BaseController
    {
        private readonly KuaforContext _context;
        public LoginController(KuaforContext context) : base(context)
        {
            _context = context;
        }
       

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]

        public IActionResult Index(string email, string sifre)
        {
            var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.Email == email && k.Sifre == sifre);

            if (kullanici != null)
            {
                // Kullanıcı ID ve Rol bilgilerini Session'a kaydet
                HttpContext.Session.SetString("UserId", kullanici.Id.ToString());
                HttpContext.Session.SetString("UserRole", kullanici.Rol);

                if (kullanici.Rol == "Admin")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.ErrorMessage = "Geçersiz e-posta veya şifre!";
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}
