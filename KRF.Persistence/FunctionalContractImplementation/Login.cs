using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRF.Common;
using KRF.Core;
using KRF.Core.FunctionalContracts;
using Dapper;
using DapperExtensions;
using StructureMap;
using KRF.Core.Entities.Actors;
using System.Configuration;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class Login:ILogin
    {
       private string _connectionString;

        public Login()
        {
            //_connectionString = ObjectFactory.GetInstance<IDatabaseConnection>().ConnectionString;
            _connectionString = Convert.ToString(ConfigurationManager.AppSettings["ApplicationDSN"]);
        }

        #region ILogin Members

        public bool AuthenticateUser(string username, string password)
        {

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                const string query = "Select U.* from [User] U inner join Employee E on E.UserID = U.ID INNER JOIN Role R on R.RoleId = E.RoleId AND R.Active = 1 where E.Status = 1 and U.Email = @Email and U.Password = @Password";
                User objUser =  sqlConnection.Query<User>(query, new { Email = username, Password = password }).FirstOrDefault();
                if (objUser != null)
                    return true;
                else
                    return false;
            }
          
        }

        public User GetUserDetail(string username)
        {

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                const string query = "Select * from [User] where Email = @Email";
                User objUser = sqlConnection.Query<User>(query, new { Email = username }).SingleOrDefault();
                return objUser;
            }

        }

        public bool ForgotPassword(string email, string code)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    const string query = "UPDATE [User] set ResetCode = @Code where Email = @Email";
                    int id = sqlConnection.Execute(query, new { Code = code, Email = email });
                    return true;
                }
            }
            catch (Exception ex)
            {
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
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                const string query = "Select * from [User] where ResetCode = @Code";
                User objUser = sqlConnection.Query<User>(query, new { Code = code }).FirstOrDefault();
                if (objUser != null)
                    return true;
                else
                    return false;
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
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    const string query = "UPDATE [User] set ResetCode = null, Password = @Password where ResetCode = @ResetCode";
                    int id = sqlConnection.Execute(query, new { Password = newPassword, ResetCode = key });
                    return id > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion
    }
}
