using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebProje.Models;

namespace WebProje.Controllers
{
    public class BaseController : Controller
    {
        public readonly KuaforContext _context;

        public BaseController(KuaforContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var magaza = _context.Magazalar.FirstOrDefault();

            ViewData["MagazaAdi"] = magaza?.Ad ?? @ViewData["MagazaAdi"];

            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            ViewData["IsLoggedIn"] = !string.IsNullOrEmpty(userId);
            ViewData["UserRole"] = userRole;

            base.OnActionExecuting(context);
        }
    }
}
