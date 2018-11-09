using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using KRF.Web.Models;
using KRF.Core.Repository.MISC;
using System.Web;
using System.Configuration;
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
            var repository = ObjectFactory.GetInstance<ILoginRepository>();
            var employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            //ILoginRepository repository;
            var username = model.UserName;
            var password = model.Password;
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                password = KRF.Common.EncryptDecrypt.EncryptString(password);
                var loginResult = repository.AuthenticateUser(username, password);
                if (ModelState.IsValid && loginResult)
                {
                    var user = repository.GetUserDetail(username);
                    SessionManager.LoggedInUser = user;
                    SessionManager.UserID = user.ID;
                    SessionManager.UserName = user.Email;

                    var rolePermissionRepo = ObjectFactory.GetInstance<IRolePermissionRepository>();
                    SessionManager.Pages = rolePermissionRepo.GetPages();

                    //Session["IsAdmin"] = user.IsAdmin;

                    var employeeDto = employeeRepo.GetEmployeeByUserID(user.ID);
                    var targetEmployee = employeeDto.Employees.FirstOrDefault();
                    if (targetEmployee == null)
                    {
                        throw new ArgumentException("Employee details are inconsistent with Login Credentials");
                    }
                    SessionManager.RoleId = targetEmployee.RoleId;
                    SessionManager.UserFullName = targetEmployee.FirstName + " " + targetEmployee.LastName;
                    SessionManager.EmpID = targetEmployee.EmpId;

                    var ticket = new FormsAuthenticationTicket(1, model.UserName, DateTime.Now, DateTime.Now.AddMinutes(30), false, string.Empty, FormsAuthentication.FormsCookiePath);
                    var encryptedCookie = FormsAuthentication.Encrypt(ticket);
                    var faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedCookie)
                    {
                        Expires = DateTime.Now.AddMinutes(
                            Convert.ToInt32(ConfigurationManager.AppSettings["UserTimeOut"])),
                        HttpOnly = true
                    };
                    //timeout in minute
                    Response.Cookies.Add(faCookie);
                    var action = "Index";
                    var controller = "Lead";
                    var rolePermissions = rolePermissionRepo.GetRolePermissionDetail(SessionManager.RoleId);
                    if (rolePermissions != null)
                    {
                        var firstAccessiblePage = rolePermissions.RolePermissions.OrderBy(p => p.PageID).FirstOrDefault();
                        if (firstAccessiblePage != null)
                        {
                            var firstAccesiblePageId = firstAccessiblePage.PageID;
                            var firstAccesiblePageName = SessionManager.Pages.Where(p => p.PageID == firstAccesiblePageId).FirstOrDefault().PageName;
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
            var success = false;
            var msg = string.Empty;
            try
            {
                var repository = ObjectFactory.GetInstance<ILoginRepository>();
                var employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
                var user = repository.GetUserDetail(email);
                if (user != null)
                {
                    var guid = Guid.NewGuid();
                    if (repository.ForgotPassword(email, guid.ToString()))
                    {
                        var employeeDto = employeeRepo.GetEmployeeByUserID(user.ID);

                        var code = KRF.Common.EncryptDecrypt.EncryptString(guid.ToString());
                        var mailSrvc = ObjectFactory.GetInstance<IMailService>();
                        var relevantEmployee = employeeDto.Employees.FirstOrDefault();
                        if (relevantEmployee == null)
                        {
                            throw new ArgumentException($"No employee corresponds to User ID {user.ID}");
                        }
                        var resetEmailModel = new ResetPasswordEmailModel
                        {
                            FirstName = relevantEmployee.FirstName,
                            LastName = relevantEmployee.LastName,
                            Email = email
                        };
                        var url = Request.UrlReferrer.ToString().Substring(0, Request.UrlReferrer.ToString().IndexOf(Request.Url.Host) + Request.Url.Host.Length);
                        resetEmailModel.ResetPasswordUrl = url + Url.Action("ResetPassword", "Account", new { key = code });
                        mailSrvc.SendMail(resetEmailModel, "ResetPassword", resetEmailModel.Email, "", "KRF - Reset Password", Server.MapPath("~/MailTemplates"), null);
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
                Console.WriteLine(ex);
                success = false;
                msg = "Some error occured. Please try again later.";
            }
            return Json(new
            {
                success,
                message = msg,
            }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string key)
        {
            var resetPasswordModel = new ResetPasswordModel();
            var repository = ObjectFactory.GetInstance<ILoginRepository>();
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
            var result = false;
            var msg = string.Empty;
            try
            {
                var repository = ObjectFactory.GetInstance<ILoginRepository>();
                var password = KRF.Common.EncryptDecrypt.EncryptString(model.NewPassword);
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
            return Json(new {result, message = msg }, JsonRequestBehavior.AllowGet);
        }

        // TODO Write AccountController.Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(ResetPasswordModel model)
        {
            var result = false;
            var msg = string.Empty;
            try
            {
            }
            catch (Exception ex)
            {
                msg = "Some error occured. Please try again";
            }
            return Json(new {result, message = msg }, JsonRequestBehavior.AllowGet);
        }

        // TODO Write AccountController.Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(ResetPasswordModel model)
        {
            var result = false;
            var msg = string.Empty;
            try
            {
            }
            catch (Exception ex)
            {
                msg = "Some error occured. Please try again";
            }
            return Json(new {result, message = msg }, JsonRequestBehavior.AllowGet);
        }

        // TODO Write AccountController.LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff(ResetPasswordModel model)
        {
            var result = false;
            var msg = string.Empty;
            try
            {
            }
            catch (Exception ex)
            {
                msg = "Some error occured. Please try again";
            }
            return Json(new {result, message = msg }, JsonRequestBehavior.AllowGet);
        }
    }
}
