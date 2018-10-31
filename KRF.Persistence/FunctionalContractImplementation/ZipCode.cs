using System.Linq;
using KRF.Core.FunctionalContracts;
using Dapper;
using KRF.Core.Entities.MISC;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class ZipCode : IZipCode
    {
        #region IZipCode Members

        public CityAndState GetCityAndState(string zipCode)
        {
            CityAndState result;
            var dbConnection = new DataAccessFactory();             using (var conn = dbConnection.CreateConnection()) 
            {
                conn.Open();
                const string query = "SELECT [City], [State] FROM ZipCode WHERE [ZipCode] = @zipCodeToFind";
                var objZipCode = conn.Query(query, new { zipCodeToFind = zipCode }).FirstOrDefault();
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
            var id = 0;

            if (cityName == null)
                return id;

            var dbConnection = new DataAccessFactory();             using (var conn = dbConnection.CreateConnection()) 
            {
                conn.Open();
                const string query = "SELECT [Id] FROM City WHERE [Description] = @cityNameParam";
                var objCity = conn.Query(query, new { cityNameParam = cityName }).FirstOrDefault();
                id = (objCity != null) ? objCity.Id : 0;
            }

            return id;
        }
        
        #endregion
    }
}
