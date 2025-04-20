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
using ServiceContracts.Enums;
using Xunit.Abstractions;
namespace CRUDTests.PersonTests
{
    public class PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
        private readonly PersonsService _personsService = new();
        private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;

        #region AddPerson
        [Fact]
        public void AddPerson_NullPersonAddRequest()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;
            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {

                //Act
                _personsService.AddPerson(personAddRequest);
            });
        }
        
        [Fact]
        public void AddPerson_NullPersonNameAddRequest()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = null
            };
            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _personsService.AddPerson(personAddRequest);
            });
        }
        
        [Fact]
        public void AddPerson_ProperPersonAddRequest()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = "Jamie",
                Address = "Casterly Rock",
                CountryID = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                Email = "Jamie@Ravens.com",
                Gender = PersonGenderEnum.Male,
                RecievesNewsLetters = true
            };
            //Act
            var personResponse = _personsService.AddPerson(personAddRequest);
            var persons = _personsService.GetAllPersons();
            //Assert
            Assert.True(personResponse.PersonID != Guid.Empty);
            Assert.Contains(personResponse, persons);
        }
        #endregion

        #region GetPersonByPersonID
        [Fact]
        public void GetPersonByPersonID_NullPersonID() {
            //Arrange
            Guid? personId = null;
            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _personsService.GetPersonByPersonID(personId);
            });
        }
        [Fact]
        public void GetPersonByPersonID_ProperPersonID()
        {
            //Arrange
            var addPersonRequest = new PersonAddRequest()
            {
                 Address = "Casterly Rock",
                 CountryID = Guid.NewGuid(),
                 DateOfBirth = DateTime.Now,
                 Gender = PersonGenderEnum.Other,
                 PersonName = "Jamie Lannister",
                 Email = "jj@ll.com",
                 RecievesNewsLetters = true
            };
            var createdPersonResponse = _personsService.AddPerson(addPersonRequest);
            //Act
            var personRetrievedFromGetPersonByPersonId = _personsService.GetPersonByPersonID(createdPersonResponse.PersonID);
            //Assert
            Assert.True(personRetrievedFromGetPersonByPersonId?.PersonID != Guid.Empty);
            Assert.Equal(personRetrievedFromGetPersonByPersonId?.PersonID , createdPersonResponse.PersonID);
        }
        [Fact]
        public void GetPersonByPersonID_NonExistingPersonWithProvidedPersonID()
        {
            //Arrange
            var nonExistingPersonId = Guid.NewGuid();
            //Act
            var person = _personsService.GetPersonByPersonID(nonExistingPersonId);
            //Assert
            Assert.Null(person);
        }

        #endregion

        #region GetAllPersons
        [Fact]
        public void GetAllPersons_EmptyList()
        {
            //Arrange
            var emptyPersonResponseList = new List<PersonResponse>();
            //Act
            var persons = _personsService.GetAllPersons();
            //Assert
            Assert.Empty(persons);
            Assert.Equal(persons, emptyPersonResponseList);
        }
        [Fact]
        public void GetAllPersons_AddFewPersons()
        {
            //Arrange
            var personsAddRequest = new List<PersonAddRequest>() {
                { new ()
            {
                PersonName = "Jamie",
                Address = "Casterly Rock",
                CountryID = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                Email = "Jamie@Ravens.com",
                Gender = PersonGenderEnum.Male,
                RecievesNewsLetters = true
            } },{
            new ()
            {
                PersonName = "John",
                Address = "Castle Black",
                CountryID = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                Email = "John@Wild.com",
                Gender = PersonGenderEnum.Male,
                RecievesNewsLetters = true
            }
            } };

            //Act
            foreach (var personAddRequest in personsAddRequest)
            {
                _personsService.AddPerson(personAddRequest);
            }
            var personsFromGetAllPersons = _personsService.GetAllPersons();
            foreach (var person in personsFromGetAllPersons)
            {
                _testOutputHelper.WriteLine($"{person}");
            }    
            //Assert
            Assert.Equal(2, personsFromGetAllPersons.Count);
        }
        #endregion

        #region GetFilteredPersons
        
        [Fact]
        public void GetFilteredPersons_EmptySearchString()
        {
            //Arrange
            var personsAddRequest = new List<PersonAddRequest>() {
            { new ()
            {
                PersonName = "Jamie",
                Address = "Casterly Rock",
                CountryID = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                Email = "Jamie@Ravens.com",
                Gender = PersonGenderEnum.Male,
                RecievesNewsLetters = true
            } },{
                new ()
                {
                    PersonName = "John",
                    Address = "Castle Black",
                    CountryID = Guid.NewGuid(),
                    DateOfBirth = DateTime.Now,
                    Email = "John@Wild.com",
                    Gender = PersonGenderEnum.Male,
                    RecievesNewsLetters = true
                }
            } };
            //Act
            foreach (var personAddRequest in personsAddRequest)
            {
                var addedPerson = _personsService.AddPerson(personAddRequest);
            }
            var personsFromGetFilteredPersons = _personsService.GetFilteredPersons(nameof(Person.PersonName) , "");
            var personsFromGetAllPersons = _personsService.GetAllPersons();
            foreach (var person in personsFromGetFilteredPersons)
            {
                _testOutputHelper.WriteLine($"{person}");
            }    
            //Assert
            Assert.Equal(personsFromGetFilteredPersons.Count, personsFromGetAllPersons.Count);
        }
        [Fact]
        public void GetFilteredPersons_ProperSearchString()
        {
            //Arrange
            var personsAddRequest = new List<PersonAddRequest>() {
            {
                new (){
                    PersonName = "Jamie",
                    Address = "Casterly Rock",
                    CountryID = Guid.NewGuid(),
                    DateOfBirth = DateTime.Now,
                    Email = "Jamie@Ravens.com",
                    Gender = PersonGenderEnum.Male,
                    RecievesNewsLetters = true
            }}
            ,{
                new (){
                    PersonName = "Jahn",
                    Address = "Castle Black",
                    CountryID = Guid.NewGuid(),
                    DateOfBirth = DateTime.Now,
                    Email = "John@Wild.com",
                    Gender = PersonGenderEnum.Male,
                    RecievesNewsLetters = true
                }}
            };
            //Act
            foreach (var personAddRequest in personsAddRequest)
            {
                _personsService.AddPerson(personAddRequest);
            }
            var personsFromGetFilteredPersons = _personsService.GetFilteredPersons(nameof(Person.PersonName) , "ja");
            foreach (var person in personsFromGetFilteredPersons)
            {
                _testOutputHelper.WriteLine($"{person}");
            }    
            //Assert
            Assert.Equal(2, personsFromGetFilteredPersons.Count);
        }
        #endregion
        
        #region GetSortedPersons
         [Fact]
         public void GetSortedPersons_PersonNameInDescendingOrder()
         {
             //Arrange
             var personsAddRequest = new List<PersonAddRequest>() {
             { new ()
             {
                 PersonName = "Jamie",
                 Address = "Casterly Rock",
                 CountryID = Guid.NewGuid(),
                 DateOfBirth = DateTime.Now,
                 Email = "Jamie@Ravens.com",
                 Gender = PersonGenderEnum.Male,
                 RecievesNewsLetters = true
             } },{
                 new ()
                 {
                     PersonName = "John",
                     Address = "Castle Black",
                     CountryID = Guid.NewGuid(),
                     DateOfBirth = DateTime.Now,
                     Email = "John@Wild.com",
                     Gender = PersonGenderEnum.Male,
                     RecievesNewsLetters = true
                 }
             } };
             // var personsFromAdd = new List<PersonResponse>();
             foreach (var addRequest in personsAddRequest)
             {
                 var addedPerson = _personsService.AddPerson(addRequest);
                 // personsFromAdd.Add(addedPerson);
             }
             var allPersons = _personsService.GetAllPersons();
             
             //Act
             var personsFromGetSortedPersons = _personsService.GetSortedPersons(allPersons , nameof(PersonResponse.PersonName) , SortOrderOptions.DESC);
             var actualSortedPersons = allPersons.OrderByDescending(p => p.PersonName).ToList();
             
             //Assert
             for (int i = 0; i < actualSortedPersons.Count; i++)
             {
                 Assert.Equal(personsFromGetSortedPersons[i], actualSortedPersons[i]);
             }

         }
         #endregion
         
        #region UpdatePerson

        [Fact]
        public void UpdatePerson_ProperPerson()
        {
            //Arrange
            var personAddRequest = new PersonAddRequest()
            {
                PersonName = "Jamie",
                Address = "Casterly Rock",
                CountryID = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                Email = "Jamie@Ravens.com",
                Gender = PersonGenderEnum.Male,
                RecievesNewsLetters = true
            };
            var person = _personsService.AddPerson(personAddRequest);
            var personUpdateRequest = new PersonUpdateRequest()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Address = "Changed Address",
            };
            //Act
            var updatedPerson = _personsService.UpdatePerson(personUpdateRequest);
            
            //Assert
            Assert.Equal(person.PersonID, updatedPerson.PersonID);
            Assert.NotEqual(person.Address , updatedPerson.Address);

        }
        [Fact]
        public void UpdatePerson_NullPerson()
        {
            //Arrange
            var personUpdateRequest = new PersonUpdateRequest()
            {
                PersonID = Guid.NewGuid(),
                Address = "Changed Address",
            };
            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                return _personsService.UpdatePerson(personUpdateRequest);
            }
                );
        }
        [Fact]
        public void UpdatePerson_InvalidPersonID()
        {
            //Arrange
            var personUpdateRequest = new PersonUpdateRequest()
            {
                PersonID = Guid.NewGuid(),
                Address = "Changed Address",
            };
            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                return _personsService.UpdatePerson(personUpdateRequest);
            }
                );
        }
        #endregion

        #region DeletePerson
        [Fact]
        public void DeletePerson_InvalidPersonID()
        {
            //Arrange
            var invalidPersonId = Guid.NewGuid();
            //Act
            var isDeleted = _personsService.DeletePerson(invalidPersonId);
            //Assert
            Assert.False(isDeleted);
        }
        [Fact]
        public void DeletePerson_ValidPersonID()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = "Jamie",
                Address = "Casterly Rock",
                CountryID = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                Email = "Jamie@Ravens.com",
                Gender = PersonGenderEnum.Male,
                RecievesNewsLetters = true
            };
            var person = _personsService.AddPerson(personAddRequest);
            //Act
            var isDeleted = _personsService.DeletePerson(person.PersonID);
            //Assert
            Assert.True(isDeleted);
        }
        [Fact]
        public void DeletePerson_NullPersonID()
        {
            //Arrange
            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _personsService.DeletePerson(null);
            });
        }

        #endregion
    }
}
