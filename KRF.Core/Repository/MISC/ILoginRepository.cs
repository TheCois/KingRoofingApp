using KRF.Core.Entities.Actors;

namespace KRF.Core.Repository.MISC
{
    public interface ILoginRepository
    {
        
        /// <summary>
        /// Authenticate user based on username and password.
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="password">user's password.</param>
        /// <returns>True - If Authentication success; Fail - If failure</returns>
        bool AuthenticateUser(string username, string password);
        /// <summary>
        /// Get User detail
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        User GetUserDetail(string username);

        /// <summary>
        /// Send user an email with ways to reset password.
        /// </summary>
        /// <param name="code">User's username</param>
        /// <returns>True - if success, False - if failure.</returns>
        bool ForgotPassword(string email, string code);

        /// <summary>
        /// Check if code is valid
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        bool IsCodeValid(string code);

        /// <summary>
        /// Reset user's password 
        /// </summary>
        /// <param name="key">User's key.</param>
        /// <param name="newPassword">New password</param>
        /// <returns>True - if success, False - if failure.</returns>
        bool ResetPassword(string key, string newPassword);

        /// <summary>
        /// Get logged in user details. Ideally call for Admin, Franchisee and related users.
        /// Use GetClientUserDetails for client and prospects instead 
        /// </summary>
        /// <param name="username">User id.</param>
        /// <returns>User details.</returns>
        User GetUserDetails(string username);

        /// <summary>
        /// Get logged in user details. Use this for client and prospects usertype. 
        /// </summary>
        /// <param name="username">User id.</param>
        /// <returns>User details.</returns>
        User GetClientUserDetails(string username);
    
    }
}
