using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebProje.Models;
using System.Linq;

namespace WebProje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KullaniciApiController : ControllerBase
    {
        private readonly KuaforContext _context;

        public KullaniciApiController(KuaforContext context)
        {
            _context = context;
        }

        [HttpGet("getKullanici/{id}")]
        public IActionResult GetKullanici(int id)
        {
            var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.Id == id);
            if (kullanici == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı!" });
            }
            return Ok(kullanici);
        }

        [HttpGet("searchKullanicilar")] // LINQ KULLANIMI
        public IActionResult SearchKullanicilar(string? rol, string? isim)
        {
            var sorgu = _context.Kullanicilar.AsQueryable(); // Sorgu Başlatma:

            //Rol Filtresi
            if (!string.IsNullOrEmpty(rol))
            {
                sorgu = sorgu.Where(k => k.Rol == rol);
            }

            //İsim Filtresi
            if (!string.IsNullOrEmpty(isim))
            {
                sorgu = sorgu.Where(k => k.Ad.Contains(isim) || k.Soyad.Contains(isim));
            }

            //Sonuçları Dönüştürme ve Listeleme
            var sonuc = sorgu.Select(k => new
            {
                k.Id,
                k.Ad,
                k.Soyad,
                k.Email,
                k.Telefon,
                k.Rol
            }).ToList();

            return Ok(sonuc);
        }


        // Tüm kullanıcıları getir
        [HttpGet("getKullanicilar")]
        public IActionResult GetKullanicilar()
        {
            var kullanicilar = _context.Kullanicilar
                .Select(k => new
                {
                    k.Id,
                    k.Ad,
                    k.Soyad,
                    k.Email,
                    k.Telefon,
                    k.Rol
                })
                .ToList();

            return Ok(kullanicilar);
        }

        // Kullanıcı ekle
        [HttpPost("addKullanici")]
        public IActionResult AddKullanici([FromBody] Kullanici kullanici)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Geçersiz kullanıcı bilgileri!" });
            }

            _context.Kullanicilar.Add(kullanici);
            _context.SaveChanges();

            return Ok(new { message = "Kullanıcı başarıyla eklendi." });
        }

        // Kullanıcı sil
        [HttpDelete("deleteKullanici/{id}")]
        public IActionResult DeleteKullanici(int id)
        {
            var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.Id == id);
            if (kullanici == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı!" });
            }

            _context.Kullanicilar.Remove(kullanici);
            _context.SaveChanges();

            return Ok(new { message = "Kullanıcı başarıyla silindi." });
        }

        // Kullanıcı güncelle
        [HttpPut("updateKullanici")]
        public IActionResult UpdateKullanici([FromBody] Kullanici kullanici)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Geçersiz kullanıcı bilgileri!" });
            }

            var mevcutKullanici = _context.Kullanicilar.FirstOrDefault(k => k.Id == kullanici.Id);
            if (mevcutKullanici == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı!" });
            }

            mevcutKullanici.Ad = kullanici.Ad;
            mevcutKullanici.Soyad = kullanici.Soyad;
            mevcutKullanici.Email = kullanici.Email;
            mevcutKullanici.Telefon = kullanici.Telefon;
            mevcutKullanici.Sifre = kullanici.Sifre;
            mevcutKullanici.Rol = kullanici.Rol;

            _context.SaveChanges();

            return Ok(new { message = "Kullanıcı başarıyla güncellendi." });
        }

        // Admin kullanıcılarını getir
        [HttpGet("getAdmins")]
        public IActionResult GetAdmins()
        {
            var admins = _context.Kullanicilar
                .Where(k => k.Rol == "Admin")
                .Select(k => new
                {
                    k.Id,
                    k.Ad,
                    k.Soyad,
                    k.Email,
                    k.Telefon
                })
                .ToList();

            return Ok(admins);
        }

        // Kullanıcı sayısı getir
        [HttpGet("getUserCount")]
        public IActionResult GetUserCount()
        {
            var count = _context.Kullanicilar.Count();
            return Ok(new { count });
        }

        // Belirli bir ID ile kullanıcı getir
     
    }
}
