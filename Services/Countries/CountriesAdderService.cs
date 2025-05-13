using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.Countries;
using ServiceContracts.DTO;
namespace Services.Countries
{
    public class CountriesAdderService(ICountriesRepository _countriesRepository) :ICountriesAdderService
    {
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }
            if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
            {
                throw new ArgumentException("There is already a Country with this name");
            }

            return (await _countriesRepository.AddCountry(countryAddRequest.ToCountry())).ToCountryResponse();
        }
    }
}
