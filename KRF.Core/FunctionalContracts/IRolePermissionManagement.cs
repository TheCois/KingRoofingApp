using System.Collections.Generic;
using KRF.Core.Entities.AccessControl;
using KRF.Core.Entities.Master;
using KRF.Core.DTO.Master;

namespace KRF.Core.FunctionalContracts
{
    public interface IRolePermissionManagement
    {
        /// <summary>
        /// Get Role Permissions
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        RolePermissionDTO GetRolePermissionDetail(int roleID);
        /// <summary>
        /// Save Role Permission
        /// </summary>
        /// <param name="rolePermissions"></param>
        /// <param name="roleID"></param>
        /// <returns></returns>
        bool SaveRolePermissions(int roleID, List<RolePermission> rolePermissions);
        /// <summary>
        /// Get Role Permission by roleID and pageID
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="pageID"></param>
        /// <returns></returns>
        List<RolePermission> GetRolePermission(int roleID, int pageID);
        /// <summary>
        /// Get Page List
        /// </summary>
        /// <returns></returns>
        List<Pages> GetPages();
    }
}
