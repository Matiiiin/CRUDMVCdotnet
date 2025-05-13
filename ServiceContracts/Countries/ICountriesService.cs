using ServiceContracts.DTO;

namespace ServiceContracts.Countries
{
    public interface ICountriesGetterService
    {
        Task<List<CountryResponse>> GetAllCountries();
        Task<CountryResponse?> GetCountryByCountryID(Guid? countryID);
    }
}
