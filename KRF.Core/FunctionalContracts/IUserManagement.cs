using System.Collections.Generic;
using KRF.Core.Entities.AccessControl;
using KRF.Core.Entities.Actors;
using KRF.Core.Entities.MISC;

namespace KRF.Core.FunctionalContracts
{
    public interface IUserManagement
    {
        /// <summary>
        /// Create a user based on filled in details
        /// </summary>
        /// <param name="user">holds user object details</param>
        /// <returns>True - if creation successful; False - if failure</returns>
        bool CreateUser(User user);

        /// <summary>
        /// Edit user information based on data updated by user.
        /// </summary>
        /// <param name="user">holds updated user object details</param>
        /// <returns>Update user object details.</returns>
        User EditUser(User user);

        /// <summary>
        /// Enables / Disabled User's status based on passed in user id.
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="tobeEnabled">True - If user id has to be enabled else false</param>
        /// <returns>True - if creation successful; False - if failure</returns>
        bool ToggleUserStatus(int userId, bool tobeEnabled);

        /// <summary>
        /// Search for a particular user based on Username
        /// </summary>
        /// <param name="username">Name of the user</param>
        /// <returns>User list</returns>
        IList<User> SearchUser(string username);

        /// <summary>
        /// Get details of user based on username (Admin, Franchisee and related users)
        /// </summary>
        /// <param name="username">Name of the user</param>
        /// <returns>user details.</returns>
        User GetUserDetails(string username);

        /// <summary>
        /// List all registered users within the system.
        /// </summary>
        /// <returns>User's list</returns>
        IList<User> ListAllUsers();

        /// <summary>
        /// Assign a set of roles to a specific user.
        /// </summary>
        /// <param name="userId">user's unique identifier</param>
        /// <param name="roles">Multiple set of roles (user shall play Admin, Franchisee owner etc)</param>
        /// <returns>True - if successful; False - If failed</returns>
        bool AssignRolesToUser(int userId, List<Role> roles);

        /// <summary>
        /// Add alternate contacts to a user record.
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="alternateContacts">ALternate contacts list</param>
        /// <returns>True - if success, False - if failure</returns>
        bool AddAlternateContacts(int userId, List<Contact> alternateContacts);
    }
}
