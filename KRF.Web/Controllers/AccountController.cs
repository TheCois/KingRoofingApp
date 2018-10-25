using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using KRF.Web.Models;
using KRF.Core.Repository.MISC;
using System.Web;
using System.Configuration;
using KRF.Core.Entities.Actors;
using KRF.Core.Repository;
using KRF.Core.FunctionalContracts;


namespace KRF.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            Session.Abandon();
            ViewBag.ReturnUrl = returnUrl;
            return View("Login");
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            ILoginRepository repository = ObjectFactory.GetInstance<ILoginRepository>();
            IEmployeeManagementRepository employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            //ILoginRepository repository;
            string username = model.UserName;
            string password = model.Password;
            if ((username != null && username != string.Empty) && (password != null && password != string.Empty))
            {
                password = KRF.Common.EncryptDecrypt.EncryptString(password);
                bool loginResult = repository.AuthenticateUser(username, password);
                if (ModelState.IsValid && loginResult)
                {
                    User user = repository.GetUserDetail(username);
                    SessionManager.LoggedInUser = user;
                    SessionManager.UserID = user.ID;
                    SessionManager.UserName = user.Email;

                    IRolePermissionRepository rolePermissionRepo = ObjectFactory.GetInstance<IRolePermissionRepository>();
                    SessionManager.Pages = rolePermissionRepo.GetPages();

                    //Session["IsAdmin"] = user.IsAdmin;

                    var employeeDTO = employeeRepo.GetEmployeByUserID(user.ID);
                    SessionManager.RoleId = employeeDTO.Employees.FirstOrDefault().RoleId;
                    SessionManager.UserFullName = employeeDTO.Employees.FirstOrDefault().FirstName + " " + employeeDTO.Employees.FirstOrDefault().LastName;
                    SessionManager.EmpID = employeeDTO.Employees.FirstOrDefault().EmpId;

                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, model.UserName, DateTime.Now, DateTime.Now.AddMinutes(30), false, string.Empty, FormsAuthentication.FormsCookiePath);
                    string encryptedCookie = FormsAuthentication.Encrypt(ticket);
                    HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedCookie);
                    faCookie.Expires = DateTime.Now.AddMinutes(Convert.ToInt32(ConfigurationManager.AppSettings["UserTimeOut"]));//timeout in minute
                    faCookie.HttpOnly = true;
                    Response.Cookies.Add(faCookie);
                    string action = "Index";
                    string controller = "Lead";
                    var rolePermissions = rolePermissionRepo.GetRolePermissionDetail(SessionManager.RoleId);
                    if (rolePermissions != null)
                    {
                        var firstAccessiblePage = rolePermissions.RolePermissions.OrderBy(p => p.PageID).FirstOrDefault();
                        if (firstAccessiblePage != null)
                        {
                            int firstAccesiblePageID = firstAccessiblePage.PageID;
                            string firstAccesiblePageName = SessionManager.Pages.Where(p => p.PageID == firstAccesiblePageID).FirstOrDefault().PageName;
                            controller = firstAccesiblePageName;
                            if (firstAccesiblePageName == "Estimate")
                            {
                                action = "EstimateIndex";
                            }
                            if (controller.ToLower() == "item" || controller.ToLower() == "assembly")
                            {
                                controller = "Product";
                            }
                        }
                    }
                    return RedirectToAction(action, controller);
                }
            }

            ModelState.AddModelError("", "Invalid credentials. Please try again");
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SendResetPassword(string email)
        {
            bool success = false;
            string msg = string.Empty;
            try
            {
                ILoginRepository repository = ObjectFactory.GetInstance<ILoginRepository>();
                IEmployeeManagementRepository employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
                User user = repository.GetUserDetail(email);
                if (user != null)
                {
                    Guid guid = Guid.NewGuid();
                    if (repository.ForgotPassword(email, guid.ToString()))
                    {
                        var employeeDTO = employeeRepo.GetEmployeByUserID(user.ID);

                        string code = KRF.Common.EncryptDecrypt.EncryptString(guid.ToString());
                        IMailService _mailSrvc = ObjectFactory.GetInstance<IMailService>();
                        KRF.Web.Models.ResetPasswordEmailModel resetEmailModel = new KRF.Web.Models.ResetPasswordEmailModel();
                        resetEmailModel.FirstName = employeeDTO.Employees.FirstOrDefault().FirstName;
                        resetEmailModel.LastName = employeeDTO.Employees.FirstOrDefault().LastName;
                        resetEmailModel.Email = email;
                        string url = Request.UrlReferrer.ToString().Substring(0, Request.UrlReferrer.ToString().IndexOf(Request.Url.Host) + Request.Url.Host.Length);
                        resetEmailModel.ResetPasswordUrl = url + Url.Action("ResetPassword", "Account", new { key = code });
                        _mailSrvc.SendMail(resetEmailModel, "ResetPassword", resetEmailModel.Email, "", "KRF - Reset Password", Server.MapPath("~/MailTemplates"), null);
                        success = true;
                        msg = "Your password is now reset. Please check your email and follow up.";
                    }
                    else
                    {
                        msg = "Email address does not exist.";
                    }
                }
                else
                {
                    msg = "Email address does not exist.";
                }
            }
            catch (Exception ex)
            {
                success = false;
                msg = "Some error occured. Please try again later.";
            }
            return Json(new
            {
                success = success,
                message = msg,
            }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string key)
        {
            ResetPasswordModel resetPasswordModel = new ResetPasswordModel();
            ILoginRepository repository = ObjectFactory.GetInstance<ILoginRepository>();
            try
            {
                key = KRF.Common.EncryptDecrypt.DecryptString(key);
                resetPasswordModel.Key = key;
                if (!repository.IsCodeValid(key))
                {
                    resetPasswordModel.IsValidKey = false;
                    return View(resetPasswordModel);
                }
                resetPasswordModel.IsValidKey = true;
            }
            catch (Exception ex)
            {
                resetPasswordModel.IsValidKey = false;
            }
            return View(resetPasswordModel);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            bool result = false;
            string msg = string.Empty;
            try
            {
                ILoginRepository repository = ObjectFactory.GetInstance<ILoginRepository>();
                string password = KRF.Common.EncryptDecrypt.EncryptString(model.NewPassword);
                if (repository.ResetPassword(model.Key, password))
                {
                    result = true;
                    msg = "Password reset successfully!";
                }
                else
                {
                    msg = "Some error occured. Please try again";
                }
            }
            catch (Exception ex)
            {
                msg = "Some error occured. Please try again";
            }
            return Json(new { result = result, message = msg }, JsonRequestBehavior.AllowGet);
        }

        // TODO Write AccountController.Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(ResetPasswordModel model)
        {
            bool result = false;
            string msg = string.Empty;
            try
            {
            }
            catch (Exception ex)
            {
                msg = "Some error occured. Please try again";
            }
            return Json(new { result = result, message = msg }, JsonRequestBehavior.AllowGet);
        }

        // TODO Write AccountController.Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(ResetPasswordModel model)
        {
            bool result = false;
            string msg = string.Empty;
            try
            {
            }
            catch (Exception ex)
            {
                msg = "Some error occured. Please try again";
            }
            return Json(new { result = result, message = msg }, JsonRequestBehavior.AllowGet);
        }

        // TODO Write AccountController.LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff(ResetPasswordModel model)
        {
            bool result = false;
            string msg = string.Empty;
            try
            {
            }
            catch (Exception ex)
            {
                msg = "Some error occured. Please try again";
            }
            return Json(new { result = result, message = msg }, JsonRequestBehavior.AllowGet);
        }
    }
}
