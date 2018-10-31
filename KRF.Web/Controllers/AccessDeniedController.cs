using System.Web.Mvc;

namespace KRF.Web.Controllers
{
    public class AccessDeniedController : Controller
    {
        public ActionResult AccessDenied(string error)
        {
            return View("AccessDenied");
        }
    }
}
