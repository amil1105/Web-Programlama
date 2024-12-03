using Microsoft.AspNetCore.Mvc;

namespace WebProje.Controllers
{
    public class AdminController : BaseController
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }
    }
}
