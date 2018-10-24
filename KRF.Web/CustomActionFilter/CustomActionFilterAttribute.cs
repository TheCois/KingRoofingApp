using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KRF.Web.CustomActionFilter
{
    public class CustomActionFilter : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string action = filterContext.ActionDescriptor.ActionName;
            if (action.ToLower() == "index" || action.ToLower() == "estimateindex")
            {
                int pageID = 0;
                if (controller.ToLower() == "product")
                {
                    pageID = SessionManager.Pages.Where(p => p.PageName == "Assembly" && p.Active == true).FirstOrDefault().PageID;
                    var assemblyEdit = KRF.Web.Common.Common.IsUserAuthorizeToPerformThisAction(KRF.Web.SessionManager.RoleId, pageID, (int)KRF.Core.Enums.PermissionType.Edit);
                    filterContext.Controller.ViewBag.AssemblyAllowEdit = assemblyEdit;
                    var assemblyView = KRF.Web.Common.Common.IsUserAuthorizeToPerformThisAction(KRF.Web.SessionManager.RoleId, pageID, (int)KRF.Core.Enums.PermissionType.View);
                    filterContext.Controller.ViewBag.AssemblyAllowView = assemblyView;
                    pageID = SessionManager.Pages.Where(p => p.PageName == "Item" && p.Active == true).FirstOrDefault().PageID;
                    var itemEdit = KRF.Web.Common.Common.IsUserAuthorizeToPerformThisAction(KRF.Web.SessionManager.RoleId, pageID, (int)KRF.Core.Enums.PermissionType.Edit);
                    filterContext.Controller.ViewBag.ItemAllowEdit = itemEdit;
                    var itemView = KRF.Web.Common.Common.IsUserAuthorizeToPerformThisAction(KRF.Web.SessionManager.RoleId, pageID, (int)KRF.Core.Enums.PermissionType.View);
                    filterContext.Controller.ViewBag.ItemAllowView = itemView;
                    if (!itemView && !itemEdit && !assemblyView && !assemblyEdit)
                    {
                        filterContext.Result = new RedirectToRouteResult(
                                                       new RouteValueDictionary 
                                   {
                                       { "action", "AccessDenied" },
                                       { "controller", "AccessDenied" }
                                   });
                    }
                }
                var page = SessionManager.Pages.Where(p => p.PageName == controller && p.Active == true).FirstOrDefault();
                if (page != null)
                {
                    pageID = page.PageID;

                    if (!Common.Common.IsUserHasBothViewAndEditPermission(SessionManager.RoleId, pageID))
                    {
                        filterContext.Result = new RedirectToRouteResult(
                                                   new RouteValueDictionary 
                                   {
                                       { "action", "AccessDenied" },
                                       { "controller", "AccessDenied" }
                                   });
                    }
                    //Check Edit Permission
                    filterContext.Controller.ViewBag.AllowEdit = KRF.Web.Common.Common.IsUserAuthorizeToPerformThisAction(KRF.Web.SessionManager.RoleId, pageID, (int)KRF.Core.Enums.PermissionType.Edit);
                }
            }
            else if (action.ToLower() == "add" || action.ToLower().Contains("save") || action.ToLower().Contains("update") || action.ToLower().Contains("import"))
            {
                int pageID = 0;
                var page = SessionManager.Pages.Where(p => p.PageName == controller && p.Active == true).FirstOrDefault();
                if (page != null)
                {
                    pageID = page.PageID;
                    if (!KRF.Web.Common.Common.IsUserAuthorizeToPerformThisAction(KRF.Web.SessionManager.RoleId, pageID, (int)KRF.Core.Enums.PermissionType.Edit))
                    {
                        filterContext.Result = new RedirectToRouteResult(
                                                   new RouteValueDictionary 
                                   {
                                       { "action", "AccessDenied" },
                                       { "controller", "AccessDenied" }
                                   });
                    }
                }
            }
            this.OnActionExecuting(filterContext);
        }
    }
}