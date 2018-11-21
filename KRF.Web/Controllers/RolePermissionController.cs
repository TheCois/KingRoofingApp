using KRF.Core.DTO.Master;
using KRF.Core.Entities.AccessControl;
using KRF.Core.Entities.MISC;
using KRF.Core.Enums;
using KRF.Core.Repository;
using KRF.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KRF.Web.Controllers
{
    public class RolePermissionController : BaseController
    {
        private const string ADMINMANAGER_ROLENAME = "Admin Manager";
        //
        // GET: /Fleet/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Get list of roles
        /// </summary>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles()
        {
            var administrationRepo = ObjectFactory.GetInstance<IAdministrationRepository>();
            var roles = administrationRepo.GetMasterRecordsByType((int)AdministrationTypes.Role);
            return Json(new
            {
                roles = roles.Where(p => p.Description != ADMINMANAGER_ROLENAME && p.Active == true).OrderBy(p=>p.Description) //"Admin Manager" is fixed name which cannot be changed through application
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Fleet Details
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult GetRolePermissions(jQueryDataTableParamModel param, int roleID)
        {
            var rolePermissionRepo = ObjectFactory.GetInstance<IRolePermissionRepository>();
            var rolePermissionDTO = new RolePermissionDTO();
            rolePermissionDTO = rolePermissionRepo.GetRolePermissionDetail(roleID);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = (from p in rolePermissionDTO.Pages.Where(p => p.Active == true)
                          select new string[] { p.PageName, (rolePermissionDTO.RolePermissions.Where(rp=>rp.RoleID == roleID && rp.PageID == p.PageID && rp.PermissionID == (int)PermissionType.View).FirstOrDefault() != null ? "<input id='"+ p.PageID  +"_"+ (int)PermissionType.View +"' type='checkbox' class='view' checked>" : "<input  id='"+ p.PageID  +"_"+ (int)PermissionType.View +"' type='checkbox' class='view' />"), 
                              (rolePermissionDTO.RolePermissions.Where(rp=>rp.RoleID == roleID && rp.PageID == p.PageID && rp.PermissionID == (int)PermissionType.Edit).FirstOrDefault() != null ? "<input  id='"+ p.PageID  +"_"+ (int)PermissionType.Edit +"' type='checkbox' class='edit' checked>" : "<input  id='"+ p.PageID  +"_"+ (int)PermissionType.Edit +"' type='checkbox' class='edit' />")}).ToArray()
            }, JsonRequestBehavior.AllowGet);
        }
        
        [ValidateAntiForgeryToken]
        public ActionResult SaveRolePermissions(int roleID, string permissions)
        {
            var rolePermissionRepo = ObjectFactory.GetInstance<IRolePermissionRepository>();
            var message = string.Empty;
            var success = false;
            var rolePerms = new List<RolePermission>();
            var pagePermissions = permissions.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
            foreach(var pagePermission in pagePermissions)
            {
                if (string.IsNullOrEmpty(pagePermission))
                    continue;
                var perms = pagePermission.Split('_');
                var rolePerm = new RolePermission();
                rolePerm.RoleID = roleID;
                rolePerm.PageID = int.Parse(perms[0]);
                rolePerm.PermissionID = int.Parse(perms[1]);
                rolePerm.DateCreated = DateTime.Now;
                rolePerms.Add(rolePerm);
            }
            rolePerms = rolePerms.OrderBy(p => p.PageID).ThenBy(q=>q.PermissionID).ToList();

            try
            {
                if (rolePermissionRepo.SaveRolePermissions(roleID, rolePerms))
                {
                    success = true;
                    message = "Role permission updated successfully.";
                }
                else
                {
                    success = false;
                    message = "Role permission could not be updated.";
                }
            }
            catch (Exception ex)
            {
                success = false;
                message = "Role permission could not be updated.";
            }

            return Json(new
            {
                success = success, message = message
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
