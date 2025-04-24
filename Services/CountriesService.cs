using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ApplicationDbContext _db;

        public CountriesService(ApplicationDbContext db)
        {
            _db = db;
        }
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
            if (await _db.Countries.Where(c => c.CountryName == countryAddRequest.CountryName).AnyAsync())
            {
                throw new ArgumentException("There is already a Country with this name");
            }
            var country = countryAddRequest.ToCountry();
            await _db.Countries.AddAsync(country);

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return await _db.Countries.Select(c => c.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
            {
                return null;
            }
            return await _db.Countries.Where(c => c.CountryID == countryID).Select(c=>c.ToCountryResponse()).FirstOrDefaultAsync();
        }


    }
}
