using KRF.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KRF.Web.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult HttpError404(string error)
        {
            ErrorModel errorModel = new ErrorModel();
            errorModel.Title = "Sorry, an error occurred while processing your request. (404)";
            errorModel.Description = error;
            return View("Error", errorModel);
        }

        public ActionResult HttpError500(string error)
        {
            ErrorModel errorModel = new ErrorModel();
            errorModel.Title = "Sorry, an error occurred while processing your request. (500)";
            errorModel.Description = error;
            return View("Error", errorModel);
        }


        public ActionResult General(string error)
        {
            ErrorModel errorModel = new ErrorModel();
            errorModel.Title = "Sorry, an error occurred while processing your request.";
            errorModel.Description = error;
            return View("Error", errorModel);
        }

    }
}
