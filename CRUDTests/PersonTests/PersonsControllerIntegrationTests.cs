using System.Net.Http.Json;
using FluentAssertions;
using HtmlAgilityPack;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace CRUDTests.PersonTests;

public class PersonsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;

    public PersonsControllerIntegrationTests(CustomWebApplicationFactory factory , ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
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
    }

    #endregion

}