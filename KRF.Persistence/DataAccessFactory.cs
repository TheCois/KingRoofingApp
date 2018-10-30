using System.Configuration;
using System.Data;
using DapperExtensions.Sql;
using KRF.Common;
using KRF.Core;
using MySql.Data.MySqlClient;

namespace KRF.Persistence
{
    public class DataAccessFactory : IDataAccessFactory
    {
        private readonly SecretsReader sReader_;

        public DataAccessFactory()
        {
            sReader_ = new SecretsReader(ConfigurationManager.AppSettings["ThirdPartySecretsFile"]);
        }

        public string ConnectionString
        {
            get
            {
                var connectionString = ConfigurationManager.ConnectionStrings["MySqlStore"].ConnectionString;
                return string.Format(connectionString, sReader_["RdsPassword"]);
            }
        }

        public IDbConnection CreateConnection()
        {
            DapperExtensions.DapperExtensions.SqlDialect = new MySqlDialect();
            return new MySqlConnection(ConnectionString);
        }
    }
}