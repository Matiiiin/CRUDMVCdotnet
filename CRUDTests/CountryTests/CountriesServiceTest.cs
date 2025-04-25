using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
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
        private readonly IFixture _fixture;
        public CountriesServiceTest()
        {
            var countriesInitialData = new List<Country>() { };
            var dbContextMock =
                new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            var dbContext = dbContextMock.Object;
            _countriesService = new CountriesService(dbContext);
            _fixture = new Fixture();
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
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>().Without(c=>c.CountryName).Create();

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
            var requests = _fixture.Build<CountryAddRequest>().With(c=>c.CountryName , "Japan").CreateMany(2);

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                {
                    //Act
                    foreach (var request in requests)
                    {
                        await _countriesService.AddCountry(request);
                    }
                });
        }
        [Fact]
        public async Task AddCountry_ProperCountryAddRequest()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Create<CountryAddRequest>();
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
            var countryAddRequests =_fixture.CreateMany<CountryAddRequest>(2);

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
            var countryAddRequest = _fixture.Create<CountryAddRequest>();
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
