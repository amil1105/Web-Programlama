using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProje.Models;

namespace WebProje.Controllers
{
    public class AdminController : BaseController
    {
        public AdminController(KuaforContext context) : base(context)
        {
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        [HttpGet]
        public IActionResult MagazaAyar()
        {
            var magaza = _context.Magazalar.FirstOrDefault();
            return View(magaza ?? new Magaza());
        }

        [HttpPost]
        public IActionResult MagazaAyar(Magaza magaza)
        {
            var mevcutMagaza = _context.Magazalar.FirstOrDefault();
            if (mevcutMagaza == null)
            {
                _context.Magazalar.Add(magaza);
            }
            else
            {
                mevcutMagaza.Ad = magaza.Ad;
                mevcutMagaza.Adres = magaza.Adres;
                mevcutMagaza.Telefon = magaza.Telefon;
            }
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Mağaza bilgileri başarıyla güncellendi!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult CalisanGuncelle(int id)
        {
            var calisan = _context.Calisanlar.FirstOrDefault(c => c.Id == id);
            if (calisan == null)
            {
                return NotFound();
            }
            return View(calisan);
        }

        [HttpPost]
        public IActionResult CalisanGuncelle(Calisan calisan)
        {
            var mevcutCalisan = _context.Calisanlar.FirstOrDefault(c => c.Id == calisan.Id);
            if (mevcutCalisan == null)
            {
                return NotFound();
            }

            mevcutCalisan.Ad = calisan.Ad;
            mevcutCalisan.Soyad = calisan.Soyad;
            mevcutCalisan.UzmanlikAlanlari = calisan.UzmanlikAlanlari;

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Çalışan başarıyla güncellendi!";
            return RedirectToAction("CalisanListesi");
        }

        [HttpGet]
        public IActionResult IslemGuncelle(int id)
        {
            var islem = _context.Islemler.FirstOrDefault(i => i.Id == id);
            if (islem == null)
            {
                return NotFound();
            }
            return View(islem);
        }

        [HttpPost]
        public IActionResult IslemGuncelle(Islem islem)
        {
            var mevcutIslem = _context.Islemler.FirstOrDefault(i => i.Id == islem.Id);
            if (mevcutIslem == null)
            {
                return NotFound();
            }

            mevcutIslem.Ad = islem.Ad;
            mevcutIslem.Sure = islem.Sure;
            mevcutIslem.Ucret = islem.Ucret;

            _context.SaveChanges();
            TempData["SuccessMessage"] = "İşlem başarıyla güncellendi!";
            return RedirectToAction("IslemListesi");
        }

        [HttpGet]
        public IActionResult CalisanEkle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CalisanEkle(Calisan calisan)
        {
            if (ModelState.IsValid)
            {
                _context.Calisanlar.Add(calisan);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Çalışan başarıyla eklendi!";
                return RedirectToAction("CalisanListesi");
            }
            return View(calisan);
        }

        public IActionResult CalisanListesi()
        {
            var calisanlar = _context.Calisanlar.ToList();
            return View(calisanlar);
        }

        // Çalışan Silme
        public IActionResult CalisanSil(int id)
        {
            var calisan = _context.Calisanlar.FirstOrDefault(c => c.Id == id);
            if (calisan != null)
            {
                _context.Calisanlar.Remove(calisan);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Çalışan başarıyla silindi!";
            }
            return RedirectToAction("CalisanListesi");
        }

        [HttpGet]
        public IActionResult IslemEkle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult IslemEkle(Islem islem)
        {
            if (ModelState.IsValid)
            {
                _context.Islemler.Add(islem);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "İşlem başarıyla eklendi!";
                return RedirectToAction("IslemListesi");
            }
            return View(islem);
        }

        public IActionResult IslemListesi()
        {
            var islemler = _context.Islemler.ToList();
            return View(islemler);
        }

        public IActionResult IslemSil(int id)
        {
            var islem = _context.Islemler.FirstOrDefault(i => i.Id == id);
            if (islem != null)
            {
                _context.Islemler.Remove(islem);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "İşlem başarıyla silindi!";
            }
            return RedirectToAction("IslemListesi");
        }
    }
}
