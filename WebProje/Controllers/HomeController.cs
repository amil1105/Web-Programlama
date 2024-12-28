using Microsoft.AspNetCore.Mvc;
using WebProje.Models;

namespace WebProje.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(KuaforContext context) : base(context)
        {
        }

        public IActionResult Index()
        {
            var cards = _context.HomepageCards.ToList(); // Dinamik kartları çek
            return View(cards);                          // Kartları View'e gönder
        }

        [HttpGet]
        public IActionResult YapayZeka()
        {
            return View();
        }


    }
}
