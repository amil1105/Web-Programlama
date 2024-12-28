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
        public async Task<IActionResult> KullaniciYonetimi()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7289/api/KullaniciApi/");

                var response = await client.GetAsync("GetAll");
                if (response.IsSuccessStatusCode)
                {
                    var kullanicilar = await response.Content.ReadFromJsonAsync<List<Kullanici>>();
                    return View(kullanicilar);
                }
                else
                {
                   // TempData["ErrorMessage"] = "API'den kullanıcı verisi alınamadı!";
                    return View(new List<Kullanici>());
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> KullaniciGuncelle(int id)
        {
            using (var client = new HttpClient())
            {
               
                client.BaseAddress = new Uri("https://localhost:7289/api/");

                var response = await client.GetAsync($"https://localhost:7289/api/KullaniciApi/getKullanici/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var kullanici = await response.Content.ReadAsAsync<Kullanici>();
                    return View(kullanici);
                }
                else
                {
                    //TempData["ErrorMessage"] = "API'den kullanıcı verisi alınamadı!";
                    return RedirectToAction("KullaniciYonetimi");
                }
            }
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
                // Yeni mağaza oluştur
                _context.Magazalar.Add(magaza);
            }
            else
            {
                // Var olan mağaza bilgilerini güncelle
                mevcutMagaza.Ad = magaza.Ad;
                mevcutMagaza.Adres = magaza.Adres;
                mevcutMagaza.Telefon = magaza.Telefon;
                mevcutMagaza.AcilisSaati = magaza.AcilisSaati;
                mevcutMagaza.KapanisSaati = magaza.KapanisSaati;
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Mağaza bilgileri başarıyla güncellendi!";
            return RedirectToAction("MagazaAyar");
        }

        [HttpGet]
        public IActionResult CalisanGuncelle(int id)
        {
            var calisan = _context.Calisanlar
                .Include(c => c.CalisanIslemler)
                .FirstOrDefault(c => c.Id == id);

            if (calisan == null)
            {
                TempData["ErrorMessage"] = "Çalışan bulunamadı!";
                return RedirectToAction("CalisanListesi");
            }

            ViewBag.Islemler = _context.Islemler.ToList();
            return View(calisan);
        }

        [HttpPost]
        public IActionResult CalisanGuncelle(Calisan calisan, IFormFile? ProfilFoto, List<int> IslemIds, List<string> CalismaGunleri, TimeSpan CalismaBaslangicSaati, TimeSpan CalismaBitisSaati)
        {
            var existingCalisan = _context.Calisanlar
                .Include(c => c.CalisanIslemler)
                .FirstOrDefault(c => c.Id == calisan.Id);

            if (existingCalisan == null)
            {
                TempData["ErrorMessage"] = "Çalışan bulunamadı!";
                return RedirectToAction("CalisanListesi");
            }

            existingCalisan.Ad = calisan.Ad;
            existingCalisan.Soyad = calisan.Soyad;
            existingCalisan.Telefon = calisan.Telefon;
            existingCalisan.Adres = calisan.Adres;
            existingCalisan.CalismaBaslangicSaati = CalismaBaslangicSaati;
            existingCalisan.CalismaBitisSaati = CalismaBitisSaati;

            // Günleri string olarak kaydet
            existingCalisan.CalismaGunleri = string.Join(",", CalismaGunleri);

            // Profil Fotoğrafı
            if (ProfilFoto != null && ProfilFoto.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + ProfilFoto.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ProfilFoto.CopyTo(fileStream);
                }
                existingCalisan.ProfilFotoPath = "/uploads/" + uniqueFileName;
            }

            // İşlemleri güncelle
            var existingIslemler = existingCalisan.CalisanIslemler;
            _context.CalisanIslemler.RemoveRange(existingIslemler);

            if (IslemIds != null && IslemIds.Any())
            {
                foreach (var islemId in IslemIds)
                {
                    _context.CalisanIslemler.Add(new CalisanIslem
                    {
                        CalisanId = calisan.Id,
                        IslemId = islemId
                    });
                }
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Çalışan başarıyla güncellendi!";
            return RedirectToAction("CalisanListesi");
        }


        [HttpGet]
        public IActionResult Raporlar()
        {
            var calisanlar = _context.Calisanlar.ToList();
            var viewModel = new RaporViewModel
            {
                Calisanlar = calisanlar,
                BaslangicTarihi = DateTime.Today.AddDays(-7), // Varsayılan: son 7 gün
                BitisTarihi = DateTime.Today // Varsayılan: bugünün tarihi
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Raporlar(DateTime baslangicTarihi, DateTime bitisTarihi, int? calisanId)
        {
            var randevular = _context.Randevular
                .Where(r => r.Tarih >= baslangicTarihi && r.Tarih <= bitisTarihi);

            if (calisanId.HasValue)
            {
                randevular = randevular.Where(r => r.CalisanId == calisanId);
            }

            var rapor = new RaporViewModel
            {
                CalisanBazindaIslem = randevular
         .GroupBy(r => r.CalisanId)
         .Select(g => new CalisanRaporu
         {
             CalisanAdi = g.FirstOrDefault() != null
                 ? g.FirstOrDefault().Calisan.Ad + " " + g.FirstOrDefault().Calisan.Soyad
                 : "Bilinmiyor",
             IslemSayisi = g.Count(),
             ToplamKazanc = g.Sum(r => r.Islem.Ucret)
         })
         .ToList(),
                MagazaGunlukKazanc = randevular.Sum(r => r.Islem.Ucret),
                BaslangicTarihi = baslangicTarihi,
                BitisTarihi = bitisTarihi,
                SeciliCalisan = calisanId.HasValue
         ? _context.Calisanlar.FirstOrDefault(c => c.Id == calisanId)?.Ad + " " + _context.Calisanlar.FirstOrDefault(c => c.Id == calisanId)?.Soyad
         : "Tüm Çalışanlar",
                Calisanlar = _context.Calisanlar.ToList()
            };


            return View("_FiltreliRaporSonuclari", rapor);
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
        public IActionResult CalisanRandevu()
        {
            // Kullanıcı oturum kontrolü
            var kullaniciId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(kullaniciId))
            {
                TempData["ErrorMessage"] = "Oturum süresi dolmuş. Lütfen tekrar giriş yapınız.";
                return RedirectToAction("Index", "Login");
            }

            // Kullanıcı bilgisi ve rol kontrolü
            var kullanici = _context.Kullanicilar.FirstOrDefault(u => u.Id.ToString() == kullaniciId);
            if (kullanici == null || kullanici.Rol != "Calisan")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişim yetkiniz yok.";
                return RedirectToAction("Index", "Home");
            }

            // CalisanId'yi bul
            var calisan = _context.Calisanlar.FirstOrDefault(c => c.Ad == kullanici.Ad && c.Soyad == kullanici.Soyad);
            if (calisan == null)
            {
                TempData["ErrorMessage"] = "Çalışan bilgileriniz bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            // Çalışana ait randevuları getir
            var randevular = _context.Randevular
                .Include(r => r.Islem)
                .Include(r => r.Calisan)
                .Where(r => r.CalisanId == calisan.Id) // Çalışana ait randevular
                .OrderBy(r => r.Tarih)
                .ThenBy(r => r.Saat)
                .ToList();

            return View(randevular);
        }


        [HttpPost]
        public IActionResult CalisanSil(int id)
        {
            // Çalışanı bul
            var calisan = _context.Calisanlar.FirstOrDefault(c => c.Id == id);
            if (calisan == null)
            {
                TempData["ErrorMessage"] = "Çalışan bulunamadı.";
                return RedirectToAction("CalisanListesi");
            }

            // Çalışana bağlı randevular varsa uyarı ver
            var randevular = _context.Randevular.Any(r => r.CalisanId == id);
            if (randevular)
            {
                TempData["ErrorMessage"] = "Bu çalışana ait randevular olduğu için silinemez.";
                return RedirectToAction("CalisanListesi");
            }

            // Çalışanı sil
            _context.Calisanlar.Remove(calisan);

            // Kullanıcı tablosundan da kaldır
            var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.Ad == calisan.Ad && k.Soyad == calisan.Soyad && k.Telefon == calisan.Telefon.ToString());
            if (kullanici != null)
            {
                _context.Kullanicilar.Remove(kullanici);
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Çalışan başarıyla silindi.";
            return RedirectToAction("CalisanListesi");
        }

        [HttpGet]
        public IActionResult CalisanEkle()
        {
            // ViewBag ile işlemleri ve günleri gönder
            ViewBag.Islemler = _context.Islemler.ToList();
            ViewBag.Gunler = Enum.GetValues(typeof(Gunler)).Cast<Gunler>().ToList();
            return View();
        }

        [HttpPost]
        public IActionResult CalisanEkle(Calisan calisan, IFormFile? ProfilFoto, string Email, string Sifre, List<int> IslemIds, List<Gunler> CalismaGunleri, TimeSpan CalismaBaslangicSaati, TimeSpan CalismaBitisSaati)
        {
            if (ModelState.IsValid)
            {
                // Profil Fotoğrafı Yükleme
                if (ProfilFoto != null && ProfilFoto.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ProfilFoto.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        ProfilFoto.CopyTo(fileStream);
                    }
                    calisan.ProfilFotoPath = "/uploads/" + uniqueFileName;
                }
                else
                {
                    calisan.ProfilFotoPath = "/uploads/default-profile.png";
                }

                // Çalışma Günlerini String'e çevir ve ata
                if (CalismaGunleri != null && CalismaGunleri.Any())
                {
                    calisan.CalismaGunleri = string.Join(",", CalismaGunleri.Select(g => g.ToString()));
                }

                // Çalışma saatlerini ata
                calisan.CalismaBaslangicSaati = CalismaBaslangicSaati;
                calisan.CalismaBitisSaati = CalismaBitisSaati;

                // Çalışanı Ekle
                _context.Calisanlar.Add(calisan);
                _context.SaveChanges();

                // Kullanıcı Bilgilerini Ekle
                var kullanici = new Kullanici
                {
                    Ad = calisan.Ad,
                    Soyad = calisan.Soyad,
                    Email = Email,
                    Sifre = Sifre,
                    Telefon = calisan.Telefon.ToString(),
                    Rol = "Calisan"
                };

                _context.Kullanicilar.Add(kullanici);
                _context.SaveChanges();

                // Çalışanın Yapabileceği İşlemleri Kaydet
                if (IslemIds != null && IslemIds.Any())
                {
                    foreach (var islemId in IslemIds)
                    {
                        var calisanIslem = new CalisanIslem
                        {
                            CalisanId = calisan.Id,
                            IslemId = islemId
                        };
                        _context.CalisanIslemler.Add(calisanIslem);
                    }
                    _context.SaveChanges();
                }

                TempData["SuccessMessage"] = "Çalışan başarıyla eklendi!";
                return RedirectToAction("CalisanListesi");
            }

            // Hata durumunda ViewBag'leri yeniden gönder
            ViewBag.Islemler = _context.Islemler.ToList();
            ViewBag.Gunler = Enum.GetValues(typeof(Gunler)).Cast<Gunler>().ToList();
            TempData["ErrorMessage"] = "Lütfen tüm alanları doldurunuz.";
            return View(calisan);
        }


        [HttpGet]
        public IActionResult RandevuOnay()
        {
            var kullaniciId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(kullaniciId))
            {
                TempData["ErrorMessage"] = "Oturum süresi dolmuş. Lütfen tekrar giriş yapınız.";
                return RedirectToAction("Index", "Login");
            }

            // Kullanıcının Rol'ünü kontrol et ve ilgili çalışana ait randevuları getir
            var kullanici = _context.Kullanicilar.FirstOrDefault(u => u.Id.ToString() == kullaniciId);
            if (kullanici == null || kullanici.Rol != "Calisan")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişim yetkiniz yok.";
                return RedirectToAction("Index", "Home");
            }

            // CalisanId'yi al ve ona ait bekleyen randevuları getir
            var calisan = _context.Calisanlar.FirstOrDefault(c => c.Ad == kullanici.Ad && c.Soyad == kullanici.Soyad);
            if (calisan == null)
            {
                TempData["ErrorMessage"] = "Çalışan bilgileri bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var randevular = _context.Randevular
                .Include(r => r.Islem)
                .Where(r => r.CalisanId == calisan.Id && r.RandevuDurum == 0)
                .ToList();

            return View(randevular);
        }

        [HttpPost]
        public IActionResult RandevuDurumGuncelle(int randevuId, int durum)
        {
            var randevu = _context.Randevular.FirstOrDefault(r => r.Id == randevuId);
            if (randevu != null)
            {
                randevu.RandevuDurum = durum; // 1: Onaylı, 2: Reddedildi
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Randevu durumu güncellendi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Randevu bulunamadı.";
            }

            return RedirectToAction("RandevuOnay");
        }



        public IActionResult CalisanListesi()
        {
            var calisanlar = _context.Calisanlar.ToList();
            return View(calisanlar);
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



        [HttpPost]
        public IActionResult AddHomepageCard(string Title, string Description, IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                var newCard = new HomepageCard
                {
                    Title = Title,
                    Description = Description,
                    ImagePath = "/uploads/" + uniqueFileName
                };

                _context.HomepageCards.Add(newCard);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Kart başarıyla eklendi!";
            }
            else
            {
                TempData["ErrorMessage"] = "Lütfen bir resim seçiniz.";
            }

            return RedirectToAction("HomepageCard");
        }

        [HttpPost]
        public IActionResult DeleteHomepageCard(int id)
        {
            var card = _context.HomepageCards.FirstOrDefault(c => c.Id == id);
            if (card != null)
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", card.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath); // Resmi sil
                }

                _context.HomepageCards.Remove(card); // Kartı sil
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Kart başarıyla silindi!";
            }
            else
            {
                TempData["ErrorMessage"] = "Kart bulunamadı!";
            }

            return RedirectToAction("HomepageCard");
        }



        [HttpGet]
        public IActionResult HomepageCard()
        {
            var cards = _context.HomepageCards.ToList(); // Kartları veritabanından çek
            return View("HomepageCard", cards); // View'e gönder
        }


    }
}
