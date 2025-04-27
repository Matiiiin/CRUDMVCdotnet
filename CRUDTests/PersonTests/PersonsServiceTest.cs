using Entities;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace CRUDTests.PersonTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly ITestOutputHelper _testOutputHelper ;
        private readonly IFixture _fixture;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            var personsInitialData = new List<Person>() { };
            var dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);
            dbContextMock.CreateDbSetMock(temp=>temp.Persons , personsInitialData);
            var dbContext = dbContextMock.Object;
            _personsService = new PersonsService(null);
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
                await _personsService.AddPerson(personAddRequest);
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
                await _personsService.AddPerson(personAddRequest);
            };
            
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }
        
        [Fact]
        public async Task AddPerson_ProperPersonAddRequest()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(p=>p.Email , "John@gmail.com").Create();
            //Act
            var personResponse =await _personsService.AddPerson(personAddRequest);
            var persons =await _personsService.GetAllPersons();
            //Assert
            
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
                await _personsService.GetPersonByPersonID(personId);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }
        [Fact]
        public async Task GetPersonByPersonID_ProperPersonID()
        {
            //Arrange
            var addPersonRequest = _fixture.Build<PersonAddRequest>().With(p=>p.Email , "John@gmail.com").Create();
            var createdPersonResponse = await _personsService.AddPerson(addPersonRequest);
            
            //Act
            var personRetrievedFromGetPersonByPersonId = await _personsService.GetPersonByPersonID(createdPersonResponse.PersonID);
            
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
            var person = await _personsService.GetPersonByPersonID(nonExistingPersonId);
            //Assert
            person.Should().BeNull();
        }

        #endregion

        #region GetAllPersons
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            //Arrange
            var emptyPersonResponseList = new List<PersonResponse>();
            //Act
            var persons = await _personsService.GetAllPersons();
            //Assert
            persons.Should().BeEmpty();
            persons.Should().BeEquivalentTo(emptyPersonResponseList);
        }
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            //Arrange
            var personsAddRequest = _fixture.Build<PersonAddRequest>().With(p=>p.Email , "John@gmail.com").CreateMany(2);

            //Act
            foreach (var personAddRequest in personsAddRequest)
            {
                await _personsService.AddPerson(personAddRequest);
            }
            var personsFromGetAllPersons = await _personsService.GetAllPersons();
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

            //Act
            foreach (var personAddRequest in personsAddRequest)
            {
                var addedPerson = await _personsService.AddPerson(personAddRequest);
            }
            var personsFromGetFilteredPersons =await _personsService.GetFilteredPersons(nameof(Person.PersonName) , "");
            var personsFromGetAllPersons =await _personsService.GetAllPersons();
    
            //Assert
            personsFromGetFilteredPersons.Count.Should().Be(personsFromGetAllPersons.Count);
        }
        [Fact]
        public async Task GetFilteredPersons_ProperSearchString()
        {
            //Arrange
            var personsAddRequest = _fixture.Build<PersonAddRequest>().With(p=>p.Email , "John@gmail.com").With(p=>p.PersonName , "jahn").CreateMany(2);

            //Act
            foreach (var personAddRequest in personsAddRequest)
            {
                await _personsService.AddPerson(personAddRequest);
            }
            var personsFromGetFilteredPersons = await _personsService.GetFilteredPersons(nameof(Person.PersonName) , "ja");
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

             // var personsFromAdd = new List<PersonResponse>();
             foreach (var addRequest in personsAddRequest)
             {
                 var addedPerson = await _personsService.AddPerson(addRequest);
                 // personsFromAdd.Add(addedPerson);
             }
             var allPersons =await _personsService.GetAllPersons();
             
             //Act
             var personsFromGetSortedPersons =await _personsService.GetSortedPersons(allPersons , nameof(PersonResponse.PersonName) , SortOrderOptions.DESC);
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
            //Arrange
            var personAddRequest = _fixture.Build<PersonAddRequest>().With(p=>p.Email , "John@gmail.com").Create();

            var person = await _personsService.AddPerson(personAddRequest);
            var personUpdateRequest = _fixture.Build<PersonUpdateRequest>().With(p=>p.Email , "Jamie@Ravens.com").With(p=>p.PersonID , person.PersonID).Create();
            //Act
            var updatedPerson =await _personsService.UpdatePerson(personUpdateRequest);
            
            //Assert
            person.PersonID.Should().Be(updatedPerson.PersonID);
            person.Address.Should().NotBe(updatedPerson.Address);

        }
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            var personUpdateRequest = _fixture.Build<PersonUpdateRequest>().Without(p=>p.PersonName).Create();

            //Act
            Func<Task> action = async () =>
            {
                await _personsService.UpdatePerson(personUpdateRequest);
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
                await _personsService.UpdatePerson(personUpdateRequest);
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
            var isDeleted =await _personsService.DeletePerson(invalidPersonId);
            //Assert
            isDeleted.Should().BeFalse();
        }
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            //Arrange
            PersonAddRequest? personAddRequest =
                _fixture.Build<PersonAddRequest>().With(p => p.Email, "Jamie@Ravens.com").Create();
            var person =await _personsService.AddPerson(personAddRequest);
            //Act
            var isDeleted =await _personsService.DeletePerson(person.PersonID);
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
                await _personsService.DeletePerson(null);
            };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion
    }
}
