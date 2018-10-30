using System.Collections.Generic;
using KRF.Core.DTO.Master;
using KRF.Core.Entities.AccessControl;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;

namespace KRF.Persistence.RepositoryImplementation
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly IRolePermissionManagement rolePermissionManagement_;

        /// <summary>
        /// Constructor
        /// </summary>
        public RolePermissionRepository()
        {
            rolePermissionManagement_ = ObjectFactory.GetInstance<IRolePermissionManagement>();
        }
        /// <summary>
        /// Get Role Permission
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public RolePermissionDTO GetRolePermissionDetail(int roleId)
        {
            return rolePermissionManagement_.GetRolePermissionDetail(roleId);
        }

        /// <summary>
        /// Save Role Permission
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="rolePermissions"></param>
        /// <returns></returns>
        public bool SaveRolePermissions(int roleId, List<RolePermission> rolePermissions)
        {
            return rolePermissionManagement_.SaveRolePermissions(roleId, rolePermissions);
        }
        /// <summary>
        /// Get Role Permission by roleID and pageID
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public List<RolePermission> GetRolePermission(int roleId, int pageId)
        {
            return rolePermissionManagement_.GetRolePermission(roleId, pageId);
        }
        /// <summary>
        /// Get Page List
        /// </summary>
        /// <returns></returns>
        public List<Pages> GetPages()
        {
            return rolePermissionManagement_.GetPages();
        }
    }
}
