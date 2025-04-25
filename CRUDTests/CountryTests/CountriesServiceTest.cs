using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using Moq;

namespace CRUDTests.CountryTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        public CountriesServiceTest()
        {
            var countriesInitialData = new List<Country>() { };
            var dbContextMock =
                new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            var dbContext = dbContextMock.Object;
            _countriesService = new CountriesService(dbContext);

        }

        #region AddCountry
        [Fact]
        public async Task AddCountry_NullCountryAddRequest()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                {
                    //Act
                    await _countriesService.AddCountry(request);
                });
        }
        [Fact]
        public async Task AddCountry_NullCountryName()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = null
            };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                {
                    //Act
                    await _countriesService.AddCountry(request);
                });
        }
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
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
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                {
                    //Act
                    await _countriesService.AddCountry(request1);
                    await _countriesService.AddCountry(request2);
                });
        }
        [Fact]
        public async Task AddCountry_ProperCountryAddRequest()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "Australia"
            };
            //Act
            var response =await _countriesService.AddCountry(request);
            var countriesResponse = await _countriesService.GetAllCountries(); 
            //Assert
            Assert.NotEqual(response.CountryID, Guid.Empty);
            Assert.Contains(response, countriesResponse);
        }
        #endregion

        #region GetAllCountries
        [Fact]
        public async Task GetAllCountries_EmptyCountriesList()
        {
            //Arrange

            //Act
            var countries = await _countriesService.GetAllCountries();
            //Assert
            Assert.Empty(countries);
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            //Arrange
            var countryAddRequests = new List<CountryAddRequest>() {
                new (){CountryName="Andalos"},
                new (){CountryName="Bravvos"},
                new (){CountryName="Meereen"},
            };

            //Act
            var countryResponses = new List<CountryResponse>();
            foreach (var countryAddRequest in countryAddRequests)
            {
                var countryResponse = await _countriesService.AddCountry(countryAddRequest);
                countryResponses.Add(countryResponse);
            }

            //Assert
            var actualCountryResponses =await _countriesService.GetAllCountries();
            foreach (var countryResponse in countryResponses)
            {
                Assert.Contains(countryResponse, actualCountryResponses);
            }

        }
        #endregion

        #region GetCountryByCountryID
        [Fact]
        public async Task GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countryID = null;
            //Act
            var countryResponse = await _countriesService.GetCountryByCountryID(countryID);
            //Assert
            Assert.Null(countryResponse);
            
        }

        [Fact]

        public async Task GetCountryByCountryID_ProperCountryID()
        {
            //Arrange
            var countryAddRequest = new CountryAddRequest() { CountryName = "Dummy" };
            var createdCountry =await _countriesService.AddCountry(countryAddRequest);
            var nonExistantCountryID = Guid.NewGuid();
            //Act
            var countryFromGettingByID = await _countriesService.GetCountryByCountryID(createdCountry.CountryID);
            var nonExistantCountry = await _countriesService.GetCountryByCountryID(nonExistantCountryID);
            //Assert
            Assert.Equal(createdCountry, countryFromGettingByID);
            Assert.Null(nonExistantCountry);
        }

        #endregion
    }
}
