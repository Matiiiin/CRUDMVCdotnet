using FluentAssertions;
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

    [Fact]
    public async Task Index_ShouldReturnAllPersons()
    {
        //Arrange
        var requestUris = new string[] {"/Persons/Index" , "/"};
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
        }
    }
}