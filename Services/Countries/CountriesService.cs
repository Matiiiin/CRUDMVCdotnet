using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.Countries;
using ServiceContracts.DTO;
namespace Services.Countries
{
    public class CountriesGetterService(ICountriesRepository _countriesRepository) : ICountriesGetterService
    {
        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return (await _countriesRepository.GetAllCountries()).Select(c=>c.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
            {
                return null;
            }

            return (await _countriesRepository.GetCountryByCountryID(countryID))?.ToCountryResponse();
        }


    }
}
