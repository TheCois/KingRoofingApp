using KRF.Core.Entities.Actors;
namespace KRF.Core.FunctionalContracts
{
    public interface ILogin
    {
        /// <summary>
        /// Functionality is to validate the supplied username and password against the database records.
        /// </summary>
        /// <param name="username">Supplied username</param>
        /// <param name="password">Supplied password</param>
        /// <returns>True - If success, False - If failure</returns>
        bool AuthenticateUser(string username, string password);
        /// <summary>
        /// Get User detail
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        User GetUserDetail(string username);

        /// <summary>
        /// Fucntionality is to kickstart the Forgot Password logic (Sending emails to user's email id)
        /// </summary>
        /// <param name="code">Supplied code</param>
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
    }
}
