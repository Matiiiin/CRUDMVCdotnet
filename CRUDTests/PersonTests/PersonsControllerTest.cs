using AutoFixture;
using CRUDMVC.Controllers;
using Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Mysqlx.Crud;
using ServiceContracts;
using ServiceContracts.Countries;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using ServiceContracts.Persons;

namespace CRUDTests.PersonTests;

public class PersonsControllerTest
{
    private readonly Mock<IPersonsGetterService> _personsGetterServiceMock;
    private readonly Mock<IPersonsAdderService> _personsAdderServiceMock;
    private readonly Mock<IPersonsSorterService> _personsSorterServiceMock;
    private readonly Mock<IPersonsUpdaterService> _personsUpdaterServiceMock;
    private readonly Mock<IPersonsDeleterService> _personsDeleterServiceMock;
    
    private readonly Mock<ICountriesAdderService> _countriesAdderServiceMock;
    private readonly Mock<ICountriesGetterService> _countriesGetterServiceMock;
    private readonly PersonsController _personsController;
    private readonly IFixture _fixture;

    public PersonsControllerTest()
    {
        _fixture = new Fixture();
        _countriesGetterServiceMock = new Mock<ICountriesGetterService>();
        _countriesAdderServiceMock = new Mock<ICountriesAdderService>();
        
        _personsGetterServiceMock = new Mock<IPersonsGetterService>();
        _personsAdderServiceMock = new Mock<IPersonsAdderService>();
        _personsSorterServiceMock = new Mock<IPersonsSorterService>();
        _personsUpdaterServiceMock = new Mock<IPersonsUpdaterService>();
        _personsDeleterServiceMock = new Mock<IPersonsDeleterService>();
        _personsController = new PersonsController(
            _personsGetterServiceMock.Object ,
            _personsAdderServiceMock.Object ,
            _personsSorterServiceMock.Object ,
            _personsDeleterServiceMock.Object ,
            _personsUpdaterServiceMock.Object ,
            _countriesAdderServiceMock.Object,
            _countriesGetterServiceMock.Object
            );
    }
    #region Index
        [Fact]
        public async Task Index_ProperData_ReturnsViewObjectResult()
        {
            //Arrange
            var searchString = _fixture.Create<string>();
            var searchBy = _fixture.Create<string>();
            var sortBy = _fixture.Create<string>();
            var orderBy = _fixture.Create<SortOrderOptions>();
            var personsResponsesList = _fixture.Create<List<PersonResponse>>();
            _personsGetterServiceMock.Setup(s => s.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(personsResponsesList);        
            _personsSorterServiceMock.Setup(s=>s.GetSortedPersons(It.IsAny<List<PersonResponse>>() ,It.IsAny<string>(), It.IsAny<SortOrderOptions>())).ReturnsAsync(personsResponsesList);
            //Act
            var result = await _personsController.Index(searchString, searchBy ,sortBy, orderBy );
            
            //Assert
            result.Should().BeAssignableTo<ViewResult>();
            _personsController.ViewData.Model.Should().Be(personsResponsesList);
            _personsController.ViewData.Model.Should().BeAssignableTo<List<PersonResponse>>();
            _personsController.ViewBag.CurrentSearchString = searchString;
            _personsController.ViewBag.CurrentSearchBy = searchBy;
            _personsController.ViewBag.CurrentSortBy = sortBy;
            _personsController.ViewBag.CurrentSortOrder = orderBy;
        }
    #endregion
    #region Create

        [Fact]
        public async Task Create_ProperCountries_ReturnsViewResult()
        {
            //Arrange
            var countriesResponses = _fixture.Create<List<CountryResponse>>();
            _countriesGetterServiceMock.Setup(s => s.GetAllCountries()).ReturnsAsync(countriesResponses);
            //Act
            var result = await _personsController.Create();
            //Assert
            result.Should().BeAssignableTo<ViewResult>();
            _personsController.ViewData["Countries"].Should().BeAssignableTo<IEnumerable<SelectListItem>>();
        }
    #endregion
    #region Store

    [Fact]
    public async Task Store_InvalidPersonAddRequest_ReturnsCreateViewResultWithErrors()
    {
        //Arrange
        var countries = _fixture.Create<List<CountryResponse>>();
        var invalidPersonAddRequest = _fixture.Build<PersonAddRequest>().Without(p => p.PersonName).Create();
        _personsController.ModelState.AddModelError("PersonName", "Person name is Required");        
        _countriesGetterServiceMock.Setup(s => s.GetAllCountries()).ReturnsAsync(countries);
        //Act
        var result = await _personsController.Store(invalidPersonAddRequest);
        
        //Assert
        result.Should().BeOfType<ViewResult>();
        _personsController.ViewData.ModelState.IsValid.Should().BeFalse();
        _personsController.ViewData["Countries"].Should().BeAssignableTo<IEnumerable<SelectListItem>>();
    }
    
    [Fact]
    public async Task Store_ValidPersonAddRequest_ReturnsRedirectToActionResultToIndexView()
    {
        //Arrange
        var validPersonAddRequest = _fixture.Create<PersonAddRequest>();
        _personsAdderServiceMock.Setup(p => p.AddPerson(It.IsAny<PersonAddRequest>()))
            .ReturnsAsync(validPersonAddRequest.ToPerson().ToPersonResponse());
        //Act
        var result = await _personsController.Store(validPersonAddRequest);
        
        //Assert
        result.Should().BeOfType<RedirectToActionResult>();
        _personsController.ViewData.ModelState.IsValid.Should().BeTrue();
    }


    #endregion
    #region Edit
    
    [Fact]
    public async Task Edit_ValidPersonID_ReturnsViewResult()
    {
        //Arrange
        var person = _fixture.Build<Person>().With(p=> p.Gender , PersonGenderEnum.Male.ToString()).Create();
        var validPersonID =person.PersonID;
        var countries = _fixture.Create<List<CountryResponse>>();
        _personsGetterServiceMock.Setup(p=>p.GetPersonByPersonID(validPersonID)).ReturnsAsync(person.ToPersonResponse());
        _countriesGetterServiceMock.Setup(s => s.GetAllCountries()).ReturnsAsync(countries);
        //Act
        var result = await _personsController.Edit(validPersonID);
        
        //Assert
        result.Should().BeOfType<ViewResult>();
        _personsController.ViewData["Countries"].Should().BeAssignableTo<IEnumerable<SelectListItem>>();
        _personsController.ViewData.Model.Should().BeAssignableTo<PersonUpdateRequest>();
    }
    #endregion
    #region Update

    [Fact]
    public async Task Update_NullPersonUpdateRequest_ReturnsBadRequest()
    {
        //Arrange
        PersonUpdateRequest? nullPersonUpdateRequest = null;
        var personID = _fixture.Create<Guid>();
        //Act
        var result = (BadRequestObjectResult)await _personsController.Update(nullPersonUpdateRequest , personID);
        //Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        result.StatusCode.Should().Be(400);
    }
    [Fact]
    public async Task Update_InvalidPersonUpdateRequest_ReturnsViewResultWithErrors()
    {
        //Arrange
        PersonUpdateRequest? invalidPersonUpdateRequestWithNullEmail = _fixture.Build<PersonUpdateRequest>().Without(request=>request.Email).With(request=>request.Gender , PersonGenderEnum.Female).Create();
        var person = _fixture.Build<Person>().With(p=>p.Gender , PersonGenderEnum.Male.ToString()).Create();
        _personsController.ModelState.AddModelError("PersonEmail", "Person email is required");
        var countries = _fixture.Create<List<CountryResponse>>();
        _personsGetterServiceMock.Setup(p=>p.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person.ToPersonResponse());
        _countriesGetterServiceMock.Setup(c=>c.GetAllCountries()).ReturnsAsync(countries);
        
        //Act
        var result = await _personsController.Update(invalidPersonUpdateRequestWithNullEmail , person.PersonID);
        
        //Assert
        _personsController.ModelState.IsValid.Should().BeFalse();
        _personsController.ViewData["Countries"].Should().BeAssignableTo<IEnumerable<SelectListItem>>();
        _personsController.ViewData.Model.Should().BeAssignableTo<PersonUpdateRequest>();
        result.Should().BeOfType<ViewResult>();
    }
    [Fact]
    public async Task Update_ProperPersonUpdateRequest_ReturnsRedirectToActionResultToIndexView()
    {
        //Arrange
        PersonUpdateRequest? personUpdateRequest = _fixture.Build<PersonUpdateRequest>().With(request=>request.Gender , PersonGenderEnum.Female).Create();
        var person = _fixture.Build<Person>().With(p=>p.Gender , PersonGenderEnum.Male.ToString()).Create();
        _personsUpdaterServiceMock.Setup(p=>p.UpdatePerson(personUpdateRequest)).ReturnsAsync(person.ToPersonResponse());        
        //Act
        var result =(RedirectToActionResult) await _personsController.Update(personUpdateRequest, person.PersonID);
        
        //Assert
        result.Should().BeOfType<RedirectToActionResult>();
        result.ActionName.Should().Be("Index");
        result.ControllerName.Should().Be("Persons");
    }
    #endregion
    #region Delete
    
    [Fact]
    public async Task Delete_ValidPersonID_ReturnsViewResult()
    {
        //Arrange
        var validPersonID = Guid.NewGuid();
        var person = _fixture.Build<Person>().With(p=>p.Gender , PersonGenderEnum.Female.ToString()).With(p=>p.PersonID , validPersonID).Create();
        _personsGetterServiceMock.Setup(p=>p.GetPersonByPersonID(validPersonID)).ReturnsAsync(person.ToPersonResponse());
        //Act
        var result = await _personsController.Delete(validPersonID) as ViewResult;
        //Assert
        result.Should().BeOfType<ViewResult>();
        result.Model.Should().Be(person.ToPersonResponse());
    }
    #endregion
    #region SubmitDelete

    [Fact]
    public async Task SubmitDelete_NullPersonInDatabase_ReturnsNotFound()
    {
        //Arrange
        var validPersonID = Guid.NewGuid();
        _personsGetterServiceMock.Setup(p=>p.GetPersonByPersonID(validPersonID)).ReturnsAsync(null as PersonResponse);
        
        //Act
        var result = await _personsController.SubmitDelete(validPersonID) as NotFoundResult;
        //Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task SubmitDelete_ValidPersonData_ReturnsRedirectToAction()
    {
        //Arrange
        var validPersonID = Guid.NewGuid();
        var person = _fixture.Build<Person>().With(p=>p.Gender , PersonGenderEnum.Female.ToString()).With(p=>p.PersonID , validPersonID).Create();
        _personsGetterServiceMock.Setup(p=>p.GetPersonByPersonID(validPersonID)).ReturnsAsync(person.ToPersonResponse());
        _personsDeleterServiceMock.Setup(p=>p.DeletePerson(validPersonID)).ReturnsAsync(true);
        
        //Act
        var result = await _personsController.SubmitDelete(validPersonID) as RedirectToActionResult;
        //Assert
        result.Should().BeOfType<RedirectToActionResult>();
        result.ActionName.Should().Be("Index");
        result.ControllerName.Should().Be("Persons");
    }
    
    #endregion
}