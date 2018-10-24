using System.Collections.Generic;
using KRF.Core.Entities.AccessControl;

namespace KRF.Core.FunctionalContracts
{
    public interface IRoleManagement
    {
        /// <summary>
        /// Create a new role based on user defined role name.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns>True - if creation successful; False - if creation step failed</returns>
        bool CreateRole(string roleName);

        /// <summary>
        /// Edit a role based on user's updated to role object
        /// </summary>
        /// <param name="role">Updated role object</param>
        /// <returns>Returns the updated role object post successful updation</returns>
        Role EditRole(Role role);

        /// <summary>
        /// Deletes role based on role id.
        /// </summary>
        /// <param name="roleId">Role's unique identifier</param>
        /// <returns>True - If deletion successful; False - If failure.</returns>
        bool DeleteRole(int roleId);

        /// <summary>
        /// List All registered roles within the system
        /// </summary>
        /// <returns>Roles list</returns>
        IList<Role> ListAllRoles();

        /// <summary>
        /// List all registered permission within the system
        /// </summary>
        /// <returns>Permission list</returns>
        IList<Permissions> ListAllPermissions();

        /// <summary>
        /// Assign permission to a specific role Id
        /// </summary>
        /// <param name="roleId">Role's unique identifier</param>
        /// <param name="permission">permission details</param>
        /// <returns>True - If successful; False - If failed</returns>
        bool AssignPermissionToRole(int roleId, Permissions permission);

        /// <summary>
        /// Search role based on user mentioned role name.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns>Role list</returns>
        IList<Role> SearchRole(string roleName);
    }
}
