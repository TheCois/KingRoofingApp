
namespace KRF.Core.Entities.AccessControl
{
    public class Role
    {
        /// <summary>
        /// Hold the unique Identifier of a particular role
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Holds the name of a role.
        /// </summary>
        public string RoleName { get; set; }

        public bool Active { get; set; }
    }
}
