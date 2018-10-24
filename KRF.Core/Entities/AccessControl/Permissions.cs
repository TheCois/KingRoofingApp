namespace KRF.Core.Entities.AccessControl
{
    public class Permissions
    {
        public int PermissionID { get; set; }

        /// <summary>
        /// Holds the name of a permission.
        /// </summary>
        public string PermissionName { get; set; }

        public bool Active { get; set; }
    }
}
