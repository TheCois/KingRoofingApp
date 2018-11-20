using KRF.Core.Entities.Actors;
using KRF.Core.Repository.MISC;
using System.Web.Mvc;
using System.Web.Routing;

namespace KRF.Web.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/

        //public ActionResult Index()
        //{
        //    return View();
        //}

        private User LoggedinUser()
        {
            return SessionManager.LoggedInUser;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (LoggedinUser() == null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    // For AJAX requests, return result as a simple string, 
                    // and inform calling JavaScript code that a user should be redirected.
                    //JsonResult result = Json("SessionTimeout", "text/html");
                    //filterContext.Result = result;
                    Response.Headers.Add("AJAX_SESSION_TIMEOUT", "1");
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary {
                        { "Controller", "Account" },
                        { "Action", "Login" }
                });
                }
            }
            base.OnActionExecuting(filterContext);
        }

        [HttpGet]
        public JsonResult GetCityAndState(string zipCode)
        {
            var repository = ObjectFactory.GetInstance<IZipCodeRepository>();
            var cityAndState = repository.GetCityAndState(zipCode);
            return Json(new { city = cityAndState.CityId, state = cityAndState.StateName }, JsonRequestBehavior.AllowGet);
        }
    }
}
