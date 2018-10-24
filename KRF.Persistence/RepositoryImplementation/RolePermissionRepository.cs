using KRF.Core.DTO.Master;
using KRF.Core.Entities.AccessControl;
using KRF.Core.Entities.Master;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;
using StructureMap;
using System.Collections.Generic;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly IRolePermissionManagement _RolePermissionManagement;

        /// <summary>
        /// Constructor
        /// </summary>
        public RolePermissionRepository()
        {
            _RolePermissionManagement = ObjectFactory.GetInstance<IRolePermissionManagement>();
        }
        /// <summary>
        /// Get Role Permission
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public RolePermissionDTO GetRolePermissionDetail(int roleID)
        {
            return _RolePermissionManagement.GetRolePermissionDetail(roleID);
        }

        /// <summary>
        /// Save Role Permission
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="rolePermissions"></param>
        /// <returns></returns>
        public bool SaveRolePermissions(int roleID, List<RolePermission> rolePermissions)
        {
            return _RolePermissionManagement.SaveRolePermissions(roleID, rolePermissions);
        }
        /// <summary>
        /// Get Role Permission by roleID and pageID
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="pageID"></param>
        /// <returns></returns>
        public List<RolePermission> GetRolePermission(int roleID, int pageID)
        {
            return _RolePermissionManagement.GetRolePermission(roleID, pageID);
        }
        /// <summary>
        /// Get Page List
        /// </summary>
        /// <returns></returns>
        public List<Pages> GetPages()
        {
            return _RolePermissionManagement.GetPages();
        }
    }
}
