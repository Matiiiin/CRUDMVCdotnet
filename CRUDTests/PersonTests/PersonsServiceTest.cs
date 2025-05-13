using Entities;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using AutoFixture;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using Moq;
using RepositoryContracts;
using Serilog.Extensions.Hosting;
using ServiceContracts.Persons;
using Services.Persons;

namespace CRUDTests.PersonTests
{
    public class PersonsServiceTest
    {
        private readonly ITestOutputHelper _testOutputHelper ;
        private readonly Mock<IPersonsRepository> _personsRepositoryMock;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IFixture _fixture;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _personsRepositoryMock = new Mock<IPersonsRepository>();
            var diagnosticContext = new DiagnosticContext(null);
            _personsAdderService = new PersonsAdderService(_personsRepositoryMock.Object , diagnosticContext);
            _personsGetterService = new PersonsGetterService(_personsRepositoryMock.Object , diagnosticContext);
            _personsUpdaterService = new PersonsUpdaterService(_personsRepositoryMock.Object , diagnosticContext);
            _personsSorterService = new PersonsSorterService(_personsRepositoryMock.Object , diagnosticContext);
            _personsDeleterService = new PersonsDeleterService(_personsRepositoryMock.Object , diagnosticContext);
            _fixture = new Fixture();

        }
        #region AddPerson
        [Fact]
        public async Task AddPerson_NullPersonAddRequest()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;
            
            //Act
            Func<Task> action = async () =>
            {
                await _personsAdderService.AddPerson(personAddRequest);
            };
            
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }
        
        [Fact]
        public async Task AddPerson_NullPersonNameAddRequest()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().Without(p=>p.PersonName).Create();
            // PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With<string?>(p => p.PersonName, (string?)null).Create();
            
            //Act
            Func<Task> action = async () =>
            {
                await _personsAdderService.AddPerson(personAddRequest);
            };
            
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }
        
        [Fact]
        public async Task AddPerson_ProperPersonAddRequest()
        {
            // Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "John@gmail.com")
                .Create();

            var person = personAddRequest.ToPerson();

            _personsRepositoryMock.Setup(r => r.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
            _personsRepositoryMock.Setup(r => r.GetAllPersons()).ReturnsAsync(new List<Person> { person });

            // Act
            var personResponse = await _personsAdderService.AddPerson(personAddRequest);
            var persons = await _personsGetterService.GetAllPersons();

            // Assert
            personResponse.PersonID.Should().NotBeEmpty();
            persons.Should().Contain(personResponse);
        }
        #endregion

        #region GetPersonByPersonID
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID() {
            //Arrange
            Guid? personId = null;
            
            //Act
            Func<Task> action = async () =>
            {
                await _personsGetterService.GetPersonByPersonID(personId);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }
        [Fact]
        public async Task GetPersonByPersonID_ProperPersonID()
        {
            //Arrange
            var addPersonRequest = _fixture.Build<PersonAddRequest>().With(p=>p.Email , "John@gmail.com").Create();
            var person = addPersonRequest.ToPerson();
            _personsRepositoryMock.Setup(r=>r.GetPersonByPersonID(person.PersonID)).ReturnsAsync(person);
            var createdPersonResponse = person.ToPersonResponse();
            
            //Act
            var personRetrievedFromGetPersonByPersonId = await _personsGetterService.GetPersonByPersonID(person.PersonID);
            
            //Assert
            personRetrievedFromGetPersonByPersonId.Should().BeEquivalentTo(createdPersonResponse);
            personRetrievedFromGetPersonByPersonId.Should().NotBe(Guid.Empty);
        }
        [Fact]
        public async Task GetPersonByPersonID_NonExistingPersonWithProvidedPersonID()
        {
            //Arrange
            var nonExistingPersonId = Guid.NewGuid();
            //Act
            var person = await _personsGetterService.GetPersonByPersonID(nonExistingPersonId);
            //Assert
            person.Should().BeNull();
        }

        #endregion

        #region GetAllPersons
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            //Arrange
            var emptyPersonResponseList = new List<PersonResponse>(){};
            var emtpyPersonsList = new List<Person>(){};
            _personsRepositoryMock.Setup(r=>r.GetAllPersons()).ReturnsAsync(emtpyPersonsList);
            
            //Act
            var persons = await _personsGetterService.GetAllPersons();
            
            //Assert
            persons.Should().BeEmpty();
            persons.Should().BeEquivalentTo(emptyPersonResponseList);
        }
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            //Arrange
            var personsAddRequest = _fixture.Build<PersonAddRequest>().With(p=>p.Email , "John@gmail.com").CreateMany(2);
            _personsRepositoryMock.Setup(r => r.GetAllPersons()).ReturnsAsync(personsAddRequest.Select(request => request.ToPerson()).ToList());
            
            //Act
            var personsFromGetAllPersons = await _personsGetterService.GetAllPersons();

            //Assert
            personsFromGetAllPersons.Count.Should().Be(2);
        }
        #endregion

        #region GetFilteredPersons
        
        [Fact]
        public async Task GetFilteredPersons_EmptySearchString()
        {
            //Arrange
            var personsAddRequest = _fixture.Build<PersonAddRequest>().With(p=>p.Email , "John@gmail.com").CreateMany(2);
            _personsRepositoryMock.Setup(r=>r.GetAllPersons()).ReturnsAsync(personsAddRequest.Select(request => request.ToPerson()).ToList());
            _personsRepositoryMock.Setup(r => r.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(personsAddRequest.Select(request => request.ToPerson()).ToList());            
            //Act
            var personsFromGetFilteredPersons =await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName) , null);
            var personsFromGetAllPersons =await _personsGetterService.GetAllPersons();
    
            //Assert
            personsFromGetFilteredPersons.Count.Should().Be(personsFromGetAllPersons.Count);
        }
        [Fact]
        public async Task GetFilteredPersons_ProperSearchString()
        {
            //Arrange
            var personsAddRequest = _fixture.Build<PersonAddRequest>().With(p=>p.Email , "John@gmail.com").With(p=>p.PersonName , "jahn").CreateMany(2);
            _personsRepositoryMock.Setup(r=>r.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(personsAddRequest.Select(request => request.ToPerson()).ToList());
            //Act
            var personsFromGetFilteredPersons = await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName) , "ja");
            
            //Assert
            personsFromGetFilteredPersons.Count.Should().Be(2);
        }
        #endregion
        
        #region GetSortedPersons
         [Fact]
         public async Task GetSortedPersons_PersonNameInDescendingOrder()
         {
             //Arrange
             var personsAddRequest = _fixture.Build<PersonAddRequest>().With(p=>p.Email , "John@gmail.com").CreateMany(2);
             _personsRepositoryMock.Setup(r=>r.GetAllPersons()).ReturnsAsync(personsAddRequest.Select(request => request.ToPerson()).ToList());
             var allPersons =await _personsGetterService.GetAllPersons();
             
             //Act
             var personsFromGetSortedPersons =await _personsSorterService.GetSortedPersons(allPersons , nameof(PersonResponse.PersonName) , SortOrderOptions.DESC);
             var actualSortedPersons = allPersons.OrderByDescending(p => p.PersonName).ToList();
             
             //Assert
             for (int i = 0; i < actualSortedPersons.Count; i++)
             {
                 personsFromGetSortedPersons.ElementAt(i).Should().Be(actualSortedPersons.ElementAt(i));
             }
             actualSortedPersons.Should().BeInDescendingOrder(temp => temp.PersonName);

         }
         #endregion
         
        #region UpdatePerson

        [Fact]
        public async Task UpdatePerson_ProperPerson()
        {
            // Arrange
            var personAddRequest = _fixture.Build<PersonAddRequest>().With(p => p.Email, "John@gmail.com").Create();
            var person = personAddRequest.ToPerson();
            _personsRepositoryMock.Setup(r => r.GetPersonByPersonID(person.PersonID)).ReturnsAsync(person);

            var personUpdateRequest = _fixture.Build<PersonUpdateRequest>()
                .With(p => p.Email, "Jamie@Ravens.com")
                .With(p => p.PersonID, person.PersonID)
                .Create();

            var updatedPerson = personUpdateRequest.ToPerson();
            _personsRepositoryMock.Setup(r => r.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(updatedPerson);

            // Act
            var result = await _personsUpdaterService.UpdatePerson(personUpdateRequest);

            // Assert
            result.PersonID.Should().Be(person.PersonID);
            result.Email.Should().Be("Jamie@Ravens.com");

        }
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            var personUpdateRequest = _fixture.Build<PersonUpdateRequest>().Without(p=>p.PersonName).Create();

            //Act
            Func<Task> action = async () =>
            {
                await _personsUpdaterService.UpdatePerson(personUpdateRequest);
            };
            
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }
        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            //Arrange
            var personUpdateRequest = _fixture.Build<PersonUpdateRequest>().With(p=>p.PersonID , Guid.NewGuid()).Create();

            //Act
            Func<Task> action = async () =>
            {
                await _personsUpdaterService.UpdatePerson(personUpdateRequest);
            };
            
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }
        #endregion

        #region DeletePerson
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Arrange
            var invalidPersonId = Guid.NewGuid();
            //Act
            var isDeleted =await _personsDeleterService.DeletePerson(invalidPersonId);
            //Assert
            isDeleted.Should().BeFalse();
        }
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            //Arrange
            PersonAddRequest? personAddRequest =
                _fixture.Build<PersonAddRequest>().With(p => p.Email, "Jamie@Ravens.com").Create();
            var person = personAddRequest.ToPerson();
            _personsRepositoryMock.Setup(r=>r.GetPersonByPersonID(person.PersonID)).ReturnsAsync(person);
            _personsRepositoryMock.Setup(r=>r.DeletePerson(person.PersonID)).ReturnsAsync(true);
            
            //Act
            var isDeleted =await _personsDeleterService.DeletePerson(person.PersonID);
            
            //Assert
            isDeleted.Should().BeTrue();
        }
        [Fact]
        public async Task DeletePerson_NullPersonID()
        {
            //Arrange
            //Assert
            Func<Task> action = async () =>
            {
                //Act
                await _personsDeleterService.DeletePerson(null);
            };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion
    }
}
