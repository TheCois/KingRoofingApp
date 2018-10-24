using KRF.Core.Entities.MISC;

namespace KRF.Core.Repository.MISC
{
    public interface IZipCodeRepository
    {
        /// <summary>
        /// Get City and State
        /// </summary>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        CityAndState GetCityAndState(string zipCode);
    }
}
