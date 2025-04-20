using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDTests.CountryTests
{
    public class CountriesServiceTest
    {
        private readonly CountriesService _countriesService;
        public CountriesServiceTest()
        {
            _countriesService = new CountriesService(false);
        }

        #region AddCountry
        [Fact]
        public void AddCountry_NullCountryAddRequest()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    //Act
                    _countriesService.AddCountry(request);
                });
        }
        [Fact]
        public void AddCountry_NullCountryName()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = null
            };

            //Assert
            Assert.Throws<ArgumentException>(
                () =>
                {
                    //Act
                    _countriesService.AddCountry(request);
                });
        }
        [Fact]
        public void AddCountry_DupliacteCountryName()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest()
            {
                CountryName = "Japan"
            };
            CountryAddRequest? request2 = new CountryAddRequest()
            {
                CountryName = "Japan"
            };

            //Assert
            Assert.Throws<ArgumentException>(
                () =>
                {
                    //Act
                    _countriesService.AddCountry(request1);
                    _countriesService.AddCountry(request2);
                });
        }
        [Fact]
        public void AddCountry_ProperCountryAddRequest()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "Australia"
            };
            //Act
            var response = _countriesService.AddCountry(request);
            var countriesResponse = _countriesService.GetAllCountries(); 
            //Assert
            Assert.NotEqual(response.CountryID, Guid.Empty);
            Assert.Contains(response, countriesResponse);
        }
        #endregion

        #region GetAllCountries
        [Fact]
        public void GetAllCountries_EmptyCountriesList()
        {
            //Arrange

            //Act
            var countries = _countriesService.GetAllCountries();
            //Assert
            Assert.Empty(countries);
        }

        [Fact]
        public void GetAllCountries_AddFewCountries()
        {
            //Arrange
            var countryAddRequests = new List<CountryAddRequest>() {
                new CountryAddRequest(){CountryName="Andalos"},
                new CountryAddRequest(){CountryName="Bravvos"},
                new CountryAddRequest(){CountryName="Meereen"},
            };

            //Act
            var countryResponses = new List<CountryResponse>();
            foreach (var countryAddRequest in countryAddRequests)
            {
                var countryResponse = _countriesService.AddCountry(countryAddRequest);
                countryResponses.Add(countryResponse);
            }

            //Assert
            var actualCountryResponses = _countriesService.GetAllCountries();
            foreach (var countryResponse in countryResponses)
            {
                Assert.Contains(countryResponse, actualCountryResponses);
            }

        }
        #endregion

        #region GetCountryByCountryID
        [Fact]
        public void GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countryID = null;
            //Act
            var countryResponse = _countriesService.GetCountryByCountryID(countryID);
            //Assert
            Assert.Null(countryResponse);
            
        }

        [Fact]

        public void GetCountryByCountryID_ProperCountryID()
        {
            //Arrange
            var countryAddRequest = new CountryAddRequest() { CountryName = "Dummy" };
            var createdCountry = _countriesService.AddCountry(countryAddRequest);
            var nonExistantCountryID = Guid.NewGuid();
            //Act
            var countryFromGettingByID = _countriesService.GetCountryByCountryID(createdCountry.CountryID);
            var nonExistantCountry = _countriesService.GetCountryByCountryID(nonExistantCountryID);
            //Assert
            Assert.Equal(createdCountry, countryFromGettingByID);
            Assert.Null(nonExistantCountry);
        }

        #endregion
    }
}
