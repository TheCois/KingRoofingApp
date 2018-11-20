using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using KRF.Core.Enums;

namespace KRF.Web.CustomActionFilter
{
    public class CustomActionFilter : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var action = filterContext.ActionDescriptor.ActionName;
            if (action.ToLower() == "index" || action.ToLower() == "estimateindex")
            {
                int pageId;
                if (controller.ToLower() == "product")
                {
                    var pages = SessionManager.Pages.FirstOrDefault(p => p.PageName == "Assembly" && p.Active);
                    if (pages != null)
                    {
                        pageId = pages.PageID;
                        var assemblyEdit = Common.Common.IsUserAuthorizeToPerformThisAction(SessionManager.RoleId,
                            pageId, (int) PermissionType.Edit);
                        filterContext.Controller.ViewBag.AssemblyAllowEdit = assemblyEdit;
                        var assemblyView = Common.Common.IsUserAuthorizeToPerformThisAction(SessionManager.RoleId,
                            pageId, (int) PermissionType.View);
                        filterContext.Controller.ViewBag.AssemblyAllowView = assemblyView;


                        var pages2 = SessionManager.Pages.FirstOrDefault(p => p.PageName == "Item" && p.Active);
                        if (pages2 != null)
                        {
                            pageId = pages2.PageID;
                            var itemEdit = Common.Common.IsUserAuthorizeToPerformThisAction(SessionManager.RoleId,
                                pageId,
                                (int) PermissionType.Edit);
                            filterContext.Controller.ViewBag.ItemAllowEdit = itemEdit;
                            var itemView = Common.Common.IsUserAuthorizeToPerformThisAction(SessionManager.RoleId,
                                pageId,
                                (int) PermissionType.View);
                            filterContext.Controller.ViewBag.ItemAllowView = itemView;
                            if (!itemView && !itemEdit && !assemblyView && !assemblyEdit)
                            {
                                filterContext.Result = new RedirectToRouteResult(
                                    new RouteValueDictionary
                                    {
                                        {"action", "AccessDenied"},
                                        {"controller", "AccessDenied"}
                                    });
                            }
                        }
                    }
                }
                var page = SessionManager.Pages.FirstOrDefault(p => p.PageName == controller && p.Active);
                if (page != null)
                {
                    pageId = page.PageID;
                    if (!Common.Common.IsUserHasBothViewAndEditPermission(SessionManager.RoleId, pageId))
                    {
                        filterContext.Result = new RedirectToRouteResult(
                                                   new RouteValueDictionary 
                                   {
                                       { "action", "AccessDenied" },
                                       { "controller", "AccessDenied" }
                                   });
                    }
                    //Check Edit Permission
                    filterContext.Controller.ViewBag.AllowEdit = Common.Common.IsUserAuthorizeToPerformThisAction(SessionManager.RoleId, pageId, (int)PermissionType.Edit);
                }
            }
            else if (action.ToLower() == "add" || action.ToLower().Contains("save") || action.ToLower().Contains("update") || action.ToLower().Contains("import"))
            {
                var page = SessionManager.Pages.FirstOrDefault(p => p.PageName == controller && p.Active);
                if (page != null)
                {
                    var pageId = page.PageID;
                    if (!Common.Common.IsUserAuthorizeToPerformThisAction(SessionManager.RoleId, pageId, (int)PermissionType.Edit))
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
            OnActionExecuting(filterContext);
        }
    }
}