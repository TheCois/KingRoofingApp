using KRF.Core.Entities.MISC;

namespace KRF.Core.FunctionalContracts
{
    public interface IZipCode
    {
        CityAndState GetCityAndState(string zipCode);
    }
}
