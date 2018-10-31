using KRF.Core.Repository;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace KRF.Web.Common
{
    public static class Common
    {
        /// <summary>
        /// Check if role has permission by roleID, pageID and permissionID
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="pageID"></param>
        /// <param name="permissionID"></param>
        /// <returns></returns>
        public static bool IsUserAuthorizeToPerformThisAction(int roleID, int pageID, int permissionID)
        {
            bool allow = true;
            if (SessionManager.RoleId != (int)Core.Enums.RoleType.AdminManager)
            {
                IRolePermissionRepository rolePermissionRepo = ObjectFactory.GetInstance<IRolePermissionRepository>();
                var rolePermissions = rolePermissionRepo.GetRolePermission(SessionManager.RoleId, pageID);
                if (!rolePermissions.Any(r => r.PermissionID == permissionID))
                {
                    allow = false;
                }
            }
            return allow;
        }
        /// <summary>
        /// Check if role has both view and edit permission by roleID and pageID
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="pageID"></param>
        /// <returns></returns>
        public static bool IsUserHasBothViewAndEditPermission(int roleID, int pageID)
        {
            bool allow = true;
            if (SessionManager.RoleId != (int)Core.Enums.RoleType.AdminManager)
            {
                IRolePermissionRepository rolePermissionRepo = ObjectFactory.GetInstance<IRolePermissionRepository>();
                var rolePermissions = rolePermissionRepo.GetRolePermission(SessionManager.RoleId, pageID);
                if (!rolePermissions.Any())
                {
                    allow = false;
                }
            }
            return allow;
        }

        public static string formatPhoneNumber(string phoneNum, string phoneFormat)
        {

            if (phoneFormat == "")
            {
                // If phone format is empty, code will use default format (###) ###-####
                phoneFormat = "(###) ###-####";
            }

            // First, remove everything except of numbers
            Regex regexObj = new Regex(@"[^\d]");
            phoneNum = regexObj.Replace(phoneNum, "");

            // Second, format numbers to phone string 
            if (phoneNum.Length > 0)
            {
                phoneNum = Convert.ToInt64(phoneNum).ToString(phoneFormat);
            }

            return phoneNum;
        }
    }
}