using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries = new List<Country>();

        public CountriesService(bool initialize = true)
        {
            if (initialize)
            {
                _countries.AddRange(new List<Country>
                {
                    new() { CountryID = Guid.Parse("4d6681c6-d6d4-4520-8b4b-9ad183ee271c"), CountryName = "Germany" },
                    new() { CountryID = Guid.Parse("ff642272-7ae8-4a19-98fc-c51b6954ec58"), CountryName = "USA" },
                    new() { CountryID = Guid.Parse("34ccdd2d-da1d-4b71-9d4d-3963a33fadaf"), CountryName = "Italy" },
                    new() { CountryID = Guid.Parse("6b93e03b-24a5-4975-81b0-39cc5832a80c"), CountryName = "Spain" },
                    new() { CountryID = Guid.Parse("54b1e29d-acc5-4a74-914e-51143301af44"), CountryName = "France" },
                    new() { CountryID = Guid.Parse("3c061c30-c967-4a1a-a2ed-8da4ac4ab918"), CountryName = "Canada" },
                    new() { CountryID = Guid.Parse("aebe6d4e-aa50-4cba-8879-91cecf7b6110"), CountryName = "Colombia" },
                });
            }
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
