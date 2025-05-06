using FluentAssertions;
using HtmlAgilityPack;
using Xunit.Abstractions;
using Xunit.Sdk;
using System.Net.Http;
using System.Net.Http.Json;
using AutoFixture;
using CRUDMVC.Controllers;
using Entities;
using Microsoft.Extensions.DependencyInjection;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;

namespace CRUDTests.PersonTests;

public class PersonsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;
    private readonly IFixture _fixture;
    private readonly IPersonsService _personsService;

    public PersonsControllerIntegrationTests(CustomWebApplicationFactory factory , ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _personsService = factory.Services.GetRequiredService<IPersonsService>();
        _fixture = new Fixture();
        _client = factory.CreateClient();
        _client.BaseAddress = new Uri("http://localhost:5114");
    }

    #region Index

    [Fact]
    public async Task Index_ShouldReturnAllPersons()
    {
        //Arrange
        var requestUris = new [] {"/Persons/Index" , "/"};
        var responses = new List<HttpResponseMessage>();
        
        //Act
        foreach (var requestUri in requestUris)
        {
            var response = await _client.GetAsync(requestUri);
            responses.Add(response);
        }
        
        //Assert
        foreach (var response in responses)
        {
            response.EnsureSuccessStatusCode();
            var html = new HtmlDocument();
            html.LoadHtml(await response.Content.ReadAsStringAsync());
            var document = html.DocumentNode;
            document.Should().NotBeNull();
            var table = document.SelectSingleNode("//table[contains(@class, 'persons')]");
            document.SelectSingleNode("//h1[contains(text(), 'Persons')]").Should().NotBeNull();

            table.Should().NotBeNull();
        }
    }

    #endregion
    #region Create

    [Fact]
    public async Task Create_ShouldReturnViewWithInputs()
    {
        // Act
        var response = await _client.GetAsync("/Persons/Create");

        // Assert
        response.EnsureSuccessStatusCode();
        var html = new HtmlDocument();
        html.LoadHtml(await response.Content.ReadAsStringAsync());
        var document = html.DocumentNode;
        document.Should().NotBeNull();
        document.SelectSingleNode("//select[@id=\"CountryID\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"PersonName\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"Email\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"DateOfBirth\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"Gender\"]").Should().NotBeNull();
        document.SelectSingleNode("//textarea[@id=\"Address\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"RecievesNewsLetters\"]").Should().NotBeNull();
        document.SelectSingleNode("//h2[contains(text(), 'Create Person')]").Should().NotBeNull();

    }

    #endregion
    
    #region Store

    [Fact]
    public async Task Store_ShouldRedirectToIndex_WhenModelIsValid()
    {
        // Arrange
        var validPerson = _fixture.Build<PersonAddRequest>().With(p=>p.Email , "test@gmail.com").Create();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(validPerson.PersonName!), "PersonName" },
            { new StringContent(validPerson.Email!), "Email" },
            { new StringContent(validPerson.DateOfBirth.ToString()!), "DateOfBirth" },
            { new StringContent(validPerson.Gender.ToString()!), "Gender" },
            { new StringContent(validPerson.CountryID.ToString()!), "CountryID" },
            { new StringContent(validPerson.Address!), "Address" },
            { new StringContent(validPerson.RecievesNewsLetters.ToString()), "RecievesNewsLetters" }
        };
        // Act
        var response = await _client.PostAsync(
            "Persons/Store",
            formData 
        );

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.EnsureSuccessStatusCode();
        var html = new HtmlDocument();
        html.LoadHtml(await response.Content.ReadAsStringAsync());
        var document = html.DocumentNode;
        document.Should().NotBeNull();
        var table = document.SelectSingleNode("//table[contains(@class, 'persons')]");
        document.SelectSingleNode("//h1[contains(text(), 'Persons')]").Should().NotBeNull();

        table.Should().NotBeNull();
        (await _personsService.GetAllPersons()).Where(p=>p.PersonName == validPerson.PersonName).Should().NotBeNull();
    }
        [Fact]
    public async Task Store_ShouldReturnViewWithErrors_WhenModelIsInvalid()
    {
        // Arrange
        var invalidPerson = _fixture.Build<PersonAddRequest>().With(p => p.Email , "asdasd").Create();
        var formData = new MultipartFormDataContent()
        {
            { new StringContent(invalidPerson.PersonName!), "PersonName" },
            { new StringContent(invalidPerson.Email!), "Email" },
            { new StringContent(invalidPerson.DateOfBirth.ToString()!), "DateOfBirth" },
            { new StringContent(invalidPerson.Gender.ToString()!), "Gender" },
            { new StringContent(invalidPerson.CountryID.ToString()!), "CountryID" },
            { new StringContent(invalidPerson.Address!), "Address" },
        };
        // Act
        var response = await _client.PostAsync("/Persons/Store", formData);

        // Assert
        response.EnsureSuccessStatusCode();
        var html = new HtmlDocument();
        html.LoadHtml(await response.Content.ReadAsStringAsync());
        var document = html.DocumentNode;
        //Check create view is displayed
        document.Should().NotBeNull();
        document.SelectSingleNode("//select[@id=\"CountryID\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"PersonName\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"Email\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"DateOfBirth\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"Gender\"]").Should().NotBeNull();
        document.SelectSingleNode("//textarea[@id=\"Address\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"RecievesNewsLetters\"]").Should().NotBeNull();
        document.SelectSingleNode("//h2[contains(text(), 'Create Person')]").Should().NotBeNull();

        response.EnsureSuccessStatusCode();

        // Check for validation error messages in the response
        var errorMessages = document.SelectNodes("//span[contains(@class, 'field-validation-error')]");
        errorMessages.Should().NotBeNull();
        errorMessages.Should().NotBeEmpty();
    }

    #endregion
    #region Edit

    [Fact]
    public async Task Edit_ShouldReturnViewWithPersonData_WhenPersonExists()
    {
        // Arrange
        var existigPerson =( await _personsService.GetAllPersons()).FirstOrDefault();

        // Act
        var response = await _client.GetAsync($"/Persons/Edit/{existigPerson!.PersonID}");

        // Assert
        response.EnsureSuccessStatusCode();
        var html = new HtmlDocument();
        html.LoadHtml(await response.Content.ReadAsStringAsync());
        var document = html.DocumentNode;
        document.SelectSingleNode("//input[@id='PersonName']").Should().NotBeNull();
        document.SelectSingleNode("//h2[contains(text(), 'Edit Person')]").Should().NotBeNull();
    }
    #endregion


}