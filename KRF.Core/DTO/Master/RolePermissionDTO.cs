using KRF.Core.Entities.AccessControl;
using System.Collections.Generic;
namespace KRF.Core.DTO.Master
{
    /// <summary>
    /// This class does not have database table. This class acts as a container for below products classes
    /// </summary>
    public class RolePermissionDTO
    {
        public IList<Role> Roles { get; set; }
        public IList<Pages> Pages { get; set; }
        public IList<Permissions> Permissions { get; set; }
        public IList<RolePermission> RolePermissions { get; set; }
    }
}
