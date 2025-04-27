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
using FluentAssertions;

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
            _countriesService = new CountriesService(null);
            _fixture = new Fixture();
        }

        #region AddCountry
        [Fact]
        public async Task AddCountry_NullCountryAddRequest()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Act
            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }
        [Fact]
        public async Task AddCountry_NullCountryName()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>().Without(c=>c.CountryName).Create();

            //Act
            Func<Task> action =
                async () =>
                {
                    await _countriesService.AddCountry(request);
                };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            //Arrange
            var requests = _fixture.Build<CountryAddRequest>().With(c=>c.CountryName , "Japan").CreateMany(2);

            //Assert
            Func<Task> action =
                async () =>
                {
                    //Act
                    foreach (var request in requests)
                    {
                        await _countriesService.AddCountry(request);
                    }
                };
            await action.Should().ThrowAsync<ArgumentException>();
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
            response.CountryID.Should().NotBe(Guid.Empty);
            countriesResponse.Should().Contain(response);
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
            countries.Should().BeEmpty();
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

            var actualCountryResponses =await _countriesService.GetAllCountries();
            
            //Assert
            actualCountryResponses.Should().BeEquivalentTo(countryResponses);

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
            countryResponse.Should().BeNull();
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
            createdCountry.Should().Be(countryFromGettingByID);
            nonExistantCountry.Should().BeNull();
        }

        #endregion
    }
}
