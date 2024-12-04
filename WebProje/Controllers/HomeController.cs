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
            return View();
        }
    }
}
