using ServiceContracts.DTO;

namespace ServiceContracts.Countries
{
    public interface ICountriesAdderService
    {
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);
    }
}
