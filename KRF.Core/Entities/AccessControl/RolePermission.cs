
using System;
namespace KRF.Core.Entities.AccessControl
{
    public class RolePermission
    {
        public int RolePermissionID { get; set; }
        public int RoleID { get; set; }
        public int PageID { get; set; }
        public int PermissionID { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
