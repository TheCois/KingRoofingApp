using System;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository.MISC;
using KRF.Core.Entities.Actors;

namespace KRF.Persistence.RepositoryImplementation
{
    public class LoginRepository:ILoginRepository 
    {
        
        private readonly ILogin _login;
        /// <summary>
        /// Constructor
        /// </summary>
        public LoginRepository()
        {
            _login = ObjectFactory.GetInstance<ILogin>();
        }

        /// <summary>
        /// Authenticate user based on username and password.
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="password">user's password.</param>
        /// <returns>User object.</returns>
        public bool AuthenticateUser(string username, string password)
        {
            return _login.AuthenticateUser(username, password);
        }

        /// <summary>
        /// Get User detail
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public User GetUserDetail(string username)
        {
            return _login.GetUserDetail(username);
        }

        /// <summary>
        /// Send user an email with ways to reset password.
        /// </summary>
        /// <param name="code">User's code</param>
        /// <returns>True - if success, False - if failure.</returns>
        public bool ForgotPassword(string email, string code)
        {
            return _login.ForgotPassword(email, code);
        }

        /// <summary>
        /// Check if code is valid
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsCodeValid(string code)
        {
            return _login.IsCodeValid(code);
        }

        /// <summary>
        /// Reset user's password 
        /// </summary>
        /// <param name="key">User's key.</param>
        /// <param name="newPassword">New password</param>
        /// <returns>True - if success, False - if failure.</returns>
        public bool ResetPassword(string key, string newPassword)
        {
            return _login.ResetPassword(key, newPassword);
        }

        /// <summary>
        /// Get logged in user details. Ideally call for Admin, Franchisee and related users.
        /// Use GetClientUserDetails for client and prospects instead 
        /// </summary>
        /// <param name="username">User id.</param>
        /// <returns>User details.</returns>
        public User GetUserDetails(string username)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get logged in user details. Use this for client and prospects usertype. 
        /// </summary>
        /// <param name="username">User id.</param>
        /// <returns>User details.</returns>
        public User GetClientUserDetails(string username)
        {
            throw new NotImplementedException();
        }
    }
}
