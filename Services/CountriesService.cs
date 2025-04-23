using Entities;
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
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }
            if (_db.Countries.Where(c => c.CountryName == countryAddRequest.CountryName).Any())
            {
                throw new ArgumentException("There is already a Country with this name");
            }
            var country = countryAddRequest.ToCountry();
            _db.Countries.Add(country);

            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _db.Countries.Select(c => c.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
            {
                return null;
            }
            return _db.Countries.Where(c => c.CountryID == countryID).Select(c=>c.ToCountryResponse()).FirstOrDefault();
        }


    }
}
