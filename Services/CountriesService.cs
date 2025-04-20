using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
namespace Services
{
    public class CountriesService : ICountriesService
    {
        // {
        //new Country() { CountryID = Guid.NewGuid() , CountryName = "Germany" },
        //new Country() { CountryID = Guid.NewGuid() , CountryName = "USA" },
        //new Country() { CountryID = Guid.NewGuid() , CountryName = "Italy" },
        //new Country() { CountryID = Guid.NewGuid() , CountryName = "Spain" },
        //new Country() { CountryID = Guid.NewGuid() , CountryName = "France" },
        //new Country() { CountryID = Guid.NewGuid() , CountryName = "Canada" },
        //new Country() { CountryID = Guid.NewGuid() , CountryName = "Colombia" },
        //}
        private readonly List<Country> _countries = new List<Country>();
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
            if (_countries.Where(c => c.CountryName == countryAddRequest.CountryName).Any())
            {
                throw new ArgumentException("There is already a Country with this name");
            }
            var country = countryAddRequest.ToCountry();
            _countries.Add(country);

            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _countries.Select(c => c.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
            {
                return null;
            }
            return _countries.Where(c => c.CountryID == countryID).Select(c=>c.ToCountryResponse()).FirstOrDefault();
        }


    }
}
