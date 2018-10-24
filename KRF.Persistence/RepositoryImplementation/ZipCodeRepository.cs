using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository.MISC;
using KRF.Persistence.FunctionalContractImplementation;
using KRF.Core.Entities.Actors;
using StructureMap;
using KRF.Core.Entities.MISC;

namespace KRF.Persistence.RepositoryImplementation
{
    public class ZipCodeRepository:IZipCodeRepository
    {
        private readonly IZipCode _zipCode;

        /// <summary>
        /// Constructor
        /// </summary>
        public ZipCodeRepository()
        {
            _zipCode = ObjectFactory.GetInstance<IZipCode>();
        }

        /// <summary>
        /// Get City and State
        /// </summary>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        public CityAndState GetCityAndState(string zipCode)
        {
            return _zipCode.GetCityAndState(zipCode);
        }
    }
}
