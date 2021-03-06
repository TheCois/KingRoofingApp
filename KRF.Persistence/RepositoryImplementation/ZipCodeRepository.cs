﻿using KRF.Core.FunctionalContracts;
using KRF.Core.Repository.MISC;
using KRF.Core.Entities.MISC;

namespace KRF.Persistence.RepositoryImplementation
{
    public class ZipCodeRepository:IZipCodeRepository
    {
        private readonly IZipCode zipCode_;

        /// <summary>
        /// Constructor
        /// </summary>
        public ZipCodeRepository()
        {
            zipCode_ = ObjectFactory.GetInstance<IZipCode>();
        }

        /// <summary>
        /// Get City and State
        /// </summary>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        public CityAndState GetCityAndState(string zipCode)
        {
            return zipCode_.GetCityAndState(zipCode);
        }
    }
}
