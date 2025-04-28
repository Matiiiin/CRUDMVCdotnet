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
using RepositoryContracts;

namespace CRUDTests.CountryTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly IFixture _fixture;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        public CountriesServiceTest()
        {
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesService = new CountriesService(_countriesRepositoryMock.Object);
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
            var countries = requests.Select(c=>c.ToCountry()).ToList();
            _countriesRepositoryMock.Setup(r=>r.GetCountryByCountryName(countries[0].CountryName)).ReturnsAsync(countries[0]);
            _countriesRepositoryMock.Setup(r=>r.GetCountryByCountryName(countries[1].CountryName)).ReturnsAsync(countries[1]);
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
            var country = request.ToCountry();
            _countriesRepositoryMock.Setup(r=>r.GetCountryByCountryName(country.CountryName)).ReturnsAsync((Country?)null);
            _countriesRepositoryMock.Setup(r=>r.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);
            _countriesRepositoryMock.Setup(r => r.GetAllCountries()).ReturnsAsync(new List<Country> { country });
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
            _countriesRepositoryMock.Setup(r => r.GetAllCountries()).ReturnsAsync(new List<Country>());
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
            var countries = countryAddRequests.Select(c=>c.ToCountry()).ToList();
            _countriesRepositoryMock.Setup(r => r.GetAllCountries()).ReturnsAsync(countries);
            //Act
            var actualCountryResponses =await _countriesService.GetAllCountries();
            
            //Assert
            actualCountryResponses.Should().BeEquivalentTo(countries);

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
            var createdCountry = countryAddRequest.ToCountry();
            var nonExistantCountryID = Guid.NewGuid();
            
            _countriesRepositoryMock.Setup(r => r.GetCountryByCountryID(createdCountry.CountryID)).ReturnsAsync(createdCountry);
            _countriesRepositoryMock.Setup(r => r.GetCountryByCountryID(nonExistantCountryID)).ReturnsAsync((Country?)null);
            
            //Act
            var countryFromGettingByID = await _countriesService.GetCountryByCountryID(createdCountry.CountryID);
            var nonExistantCountry = await _countriesService.GetCountryByCountryID(nonExistantCountryID);
            //Assert
            createdCountry.ToCountryResponse().Should().Be(countryFromGettingByID);
            nonExistantCountry.Should().BeNull();
        }

        #endregion
    }
}
