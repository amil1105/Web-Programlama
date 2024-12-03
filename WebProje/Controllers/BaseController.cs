using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebProje.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            // Oturum bilgilerini ViewData'ya ekle
            ViewData["IsLoggedIn"] = !string.IsNullOrEmpty(userId);
            ViewData["UserRole"] = userRole;

            base.OnActionExecuting(context);
        }
    }
}

