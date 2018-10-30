using System;
using System.Linq;
using KRF.Core.FunctionalContracts;
using Dapper;
using KRF.Core.Entities.Actors;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class Login : ILogin
    {
        #region ILogin Members

        public bool AuthenticateUser(string username, string password)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                const string query = "Select U.* from User U inner join Employee E on E.UserID = U.ID INNER JOIN Role R on R.RoleId = E.RoleId AND R.Active = 1 where E.Status = 1 and U.Email = @Email and U.Password = @Password";
                try
                {
                    var objUser = conn.Query<User>(query, new {Email = username, Password = password}).FirstOrDefault();
                    if (objUser != null)
                        return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return false;
            }
        }

        public User GetUserDetail(string username)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                const string query = "Select * from User where Email = @Email";
                var objUser = conn.Query<User>(query, new { Email = username }).SingleOrDefault();
                return objUser;
            }
        }

        public bool ForgotPassword(string email, string code)
        {
            try
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    const string query = "UPDATE [User] set ResetCode = @Code where Email = @Email";
                    conn.Execute(query, new { Code = code, Email = email });
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Check if code is valid
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsCodeValid(string code)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                const string query = "Select * from User where ResetCode = @Code";
                var objUser = conn.Query<User>(query, new { Code = code }).FirstOrDefault();
                return objUser != null;
            }
        }

        /// <summary>
        /// Reset user's password 
        /// </summary>
        /// <param name="key">User's key.</param>
        /// <param name="newPassword">New password</param>
        /// <returns>True - if success, False - if failure.</returns>
        public bool ResetPassword(string key, string newPassword)
        {
            try
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    const string query = "UPDATE User set ResetCode = null, Password = @Password where ResetCode = @ResetCode";
                    var id = conn.Execute(query, new { Password = newPassword, ResetCode = key });
                    return id > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        #endregion
    }
}
