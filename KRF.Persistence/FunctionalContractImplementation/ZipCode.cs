using System;
using System.Data.SqlClient;
using System.Linq;
using KRF.Core.FunctionalContracts;
using Dapper;
using System.Configuration;
using KRF.Core.Entities.MISC;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class ZipCode:IZipCode
    {
       private string _connectionString;

        public ZipCode()
        {
            //_connectionString = ObjectFactory.GetInstance<IDatabaseConnection>().ConnectionString;
            _connectionString = Convert.ToString(ConfigurationManager.AppSettings["ApplicationDSN"]);
        }

        #region IZipCode Members

        public CityAndState GetCityAndState(string zipCode)
        {
            CityAndState result = null;
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                const string query = "SELECT [City], [State] FROM ZipCode WHERE [ZipCode] = @zipCodeToFind";
                var objZipCode = sqlConnection.Query(query, new { zipCodeToFind = zipCode }).FirstOrDefault();
                result = new CityAndState
                {
                    CityId = GetCityId((objZipCode != null) ? objZipCode.City : null),
                    StateName = (objZipCode != null) ? objZipCode.State : null
                };
            }
            return result;
        }

        private int GetCityId(string cityName)
        {
            int id = 0;

            if (cityName == null)
                return id;

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                const string query = "SELECT [Id] FROM City WHERE [Description] = @cityNameParam";
                var objCity = sqlConnection.Query(query, new { cityNameParam = cityName }).FirstOrDefault();
                id = (objCity != null) ? objCity.Id : 0;
            }

            return id;
        }
        
        #endregion
    }
}
