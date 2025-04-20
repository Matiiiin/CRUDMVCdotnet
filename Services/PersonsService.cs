using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContracts.Enums;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly List<Person> _persons = new List<Person>();
        private readonly ICountriesService _countriesService = new CountriesService();

        public PersonsService(bool initialize = true)
        {
            if (initialize)
            {
                _persons.AddRange(new List<Person>()
                {
                    new()
                    {
                        PersonID = Guid.Parse("0c97d5dd-5984-436a-a1f2-2fe1f3857a59"),
                        PersonName = "Michael Johnson",
                        Email = "michael.johnson@example.com",
                        DateOfBirth = new DateTime(1985, 3, 12),
                        Gender = "Male",
                        CountryID = _countriesService.GetAllCountries()[0].CountryID,
                        Address = "123 Maple Street, New York, NY",
                        RecievesNewsLetters = true
                    },
                    new()
                    {
                        PersonID = Guid.Parse("be245ea5-9e28-4cb4-97c0-290bc619b082"),
                        PersonName = "Emily Davis",
                        Email = "emily.davis@example.com",
                        DateOfBirth = new DateTime(1992, 7, 25),
                        Gender = "Female",
                        CountryID = _countriesService.GetAllCountries()[4].CountryID,
                        Address = "456 Oak Avenue, Los Angeles, CA",
                        RecievesNewsLetters = false
                    },
                    new()
                    {
                        PersonID = Guid.Parse("e4ae92cb-76ef-4180-af85-e3117a7bf45a"),
                        PersonName = "James Smith",
                        Email = "james.smith@example.com",
                        DateOfBirth = new DateTime(1978, 11, 5),
                        Gender = "Male",
                        CountryID = _countriesService.GetAllCountries()[5].CountryID,
                        Address = "789 Pine Road, Chicago, IL",
                        RecievesNewsLetters = true
                    },
                    new()
                    {
                        PersonID = Guid.Parse("c7972b4b-c1cb-465e-948b-8c50969d56e8"),
                        PersonName = "Sophia Brown",
                        Email = "sophia.brown@example.com",
                        DateOfBirth = new DateTime(2000, 4, 18),
                        Gender = "Female",
                        CountryID = _countriesService.GetAllCountries()[1].CountryID,
                        Address = "321 Cedar Lane, Houston, TX",
                        RecievesNewsLetters = false
                    },
                    new()
                    {
                        PersonID = Guid.Parse("32cc403b-38a6-41ce-87c4-415aacab9b9d"),
                        PersonName = "William Garcia",
                        Email = "william.garcia@example.com",
                        DateOfBirth = new DateTime(1995, 9, 30),
                        Gender = "Male",
                        CountryID = _countriesService.GetAllCountries()[2].CountryID,
                        Address = "654 Birch Street, Phoenix, AZ",
                        RecievesNewsLetters = true
                    },
                    new()
                    {
                        PersonID = Guid.Parse("2c503e0b-5ae8-4248-a020-30bed949e283"),
                        PersonName = "Olivia Martinez",
                        Email = "olivia.martinez@example.com",
                        DateOfBirth = new DateTime(1988, 6, 22),
                        Gender = "Female",
                        CountryID = _countriesService.GetAllCountries()[1].CountryID,
                        Address = "987 Spruce Drive, Philadelphia, PA",
                        RecievesNewsLetters = true
                    },
                    new()
                    {
                        PersonID = Guid.Parse("5eda0c41-f885-4ec2-8a1c-68bf060cb9a2"),
                        PersonName = "Benjamin Wilson",
                        Email = "benjamin.wilson@example.com",
                        DateOfBirth = new DateTime(1990, 1, 15),
                        Gender = "Male",
                        CountryID = _countriesService.GetAllCountries()[2].CountryID,
                        Address = "159 Elm Court, San Antonio, TX",
                        RecievesNewsLetters = false
                    },
                    new()
                    {
                        PersonID = Guid.Parse("878e4edf-f877-4db5-86fa-ef37dfbe1a2f"),
                        PersonName = "Isabella Anderson",
                        Email = "isabella.anderson@example.com",
                        DateOfBirth = new DateTime(1998, 12, 10),
                        Gender = "Female",
                        CountryID = _countriesService.GetAllCountries()[3].CountryID,
                        Address = "753 Willow Way, San Diego, CA",
                        RecievesNewsLetters = true
                    },
                    new()
                    {
                        PersonID = Guid.Parse("d2f86a9c-8681-4f76-89ab-aa18ea43bbc3"),
                        PersonName = "Alexander Thomas",
                        Email = "alexander.thomas@example.com",
                        DateOfBirth = new DateTime(1983, 5, 8),
                        Gender = "Male",
                        CountryID = _countriesService.GetAllCountries()[4].CountryID,
                        Address = "852 Aspen Circle, Dallas, TX",
                        RecievesNewsLetters = false
                    },
                    new()
                    {
                        PersonID = Guid.Parse("574ae25f-2d09-4d57-8c76-56913731e0a1"),
                        PersonName = "Mia Taylor",
                        Email = "mia.taylor@example.com",
                        DateOfBirth = new DateTime(1993, 10, 20),
                        Gender = "Female",
                        CountryID = _countriesService.GetAllCountries()[6].CountryID,
                        Address = "951 Redwood Boulevard, San Jose, CA",
                        RecievesNewsLetters = true
                    }
                });
                
            }
        }
        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            var personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesService.GetCountryByCountryID(person.CountryID)?.CountryName;

            return personResponse;
        }
        
        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(PersonAddRequest));
            }


            ModelValidation.Validate(personAddRequest);

            var createdPerson = personAddRequest.ToPerson();
            _persons.Add(createdPerson);
            return ConvertPersonToPersonResponse(createdPerson);
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _persons.Select(p => ConvertPersonToPersonResponse(p)).ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }
            var person = _persons.Where(p => p.PersonID == personID)?.FirstOrDefault();
            if (person == null)
            {
                return null;
            }
            return ConvertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetFilteredPersons(string? searchBy, string? searchString)
        {
            List<PersonResponse> matchingPersons;
            var allPersons = GetAllPersons();
            if (string.IsNullOrEmpty(searchBy))
            {
                return allPersons;
            }

            switch (searchBy)
            {
                case nameof(Person.PersonName):
                    matchingPersons = allPersons.Where(p=> p.PersonName != null && p.PersonName.Contains(searchString , StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(Person.DateOfBirth):
                    matchingPersons = allPersons.Where(p => p.DateOfBirth != null && p.DateOfBirth.Value.ToString("dd/MM/yyyy").Contains(searchString)).ToList();
                    break;
                
                case nameof(Person.Gender):
                    matchingPersons = allPersons.Where(p => p.Gender != null && p.Gender.Contains(searchString)).ToList();
                    break;
                
                case nameof(Person.CountryID):
                    matchingPersons = allPersons.Where(p=> p.CountryID != null && p.Country.Contains(searchString)).ToList();
                    break;
                
                case nameof(Person.Email):
                    matchingPersons = allPersons.Where(p => p.Email != null && p.Email.Contains(searchString , StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                
                case nameof(Person.Address):
                    matchingPersons = allPersons.Where(p => p.Address != null && p.Address.Contains(searchString , StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                default:
                    matchingPersons = allPersons;
                    break;
            }
            return matchingPersons;

        }
        
        public List<PersonResponse> GetSortedPersons(List<PersonResponse> persons, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                return persons;
            }

            List<PersonResponse> sortedPersons = (sortBy, sortOrder)
                switch
                {
                    (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => persons
                        .OrderBy(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                    (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => persons
                        .OrderByDescending(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                    
                    
                    (nameof(PersonResponse.Email), SortOrderOptions.ASC) => persons
                        .OrderBy(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                    (nameof(PersonResponse.Email), SortOrderOptions.DESC) => persons
                        .OrderByDescending(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                    
                    
                    (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => persons
                        .OrderBy(p => p.DateOfBirth).ToList(),
                    (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => persons
                        .OrderByDescending(p => p.DateOfBirth).ToList(),
                    
                    
                    (nameof(PersonResponse.Age), SortOrderOptions.ASC) => persons
                        .OrderBy(p => p.Age).ToList(),
                    (nameof(PersonResponse.Age), SortOrderOptions.DESC) => persons
                        .OrderByDescending(p => p.Age).ToList(),
                    
                    
                    (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => persons
                        .OrderBy(p => p.Gender , StringComparer.OrdinalIgnoreCase).ToList(),
                    (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => persons
                        .OrderByDescending(p => p.Gender , StringComparer.OrdinalIgnoreCase).ToList(),

                    
                    (nameof(PersonResponse.Country), SortOrderOptions.ASC) => persons
                        .OrderBy(p => p.Country , StringComparer.OrdinalIgnoreCase).ToList(),
                    (nameof(PersonResponse.Country), SortOrderOptions.DESC) => persons
                        .OrderByDescending(p => p.Country , StringComparer.OrdinalIgnoreCase).ToList(),

                    
                    (nameof(PersonResponse.Address), SortOrderOptions.ASC) => persons
                        .OrderBy(p => p.Address , StringComparer.OrdinalIgnoreCase).ToList(),
                    (nameof(PersonResponse.Address), SortOrderOptions.DESC) => persons
                        .OrderByDescending(p => p.Address , StringComparer.OrdinalIgnoreCase).ToList(),
                    
                    
                    (nameof(PersonResponse.RecievesNewsLetters), SortOrderOptions.ASC) => persons
                        .OrderBy(p => p.RecievesNewsLetters).ToList(),
                    (nameof(PersonResponse.RecievesNewsLetters), SortOrderOptions.DESC) => persons
                        .OrderByDescending(p => p.RecievesNewsLetters).ToList(),
                    
                    _ => persons
                };
            return sortedPersons;
        }

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(personUpdateRequest));
            }
            ModelValidation.Validate(personUpdateRequest);
            
            var person = _persons.FirstOrDefault(p=>p.PersonID == personUpdateRequest.PersonID);
            if (person == null)
            {
                throw new ArgumentException("Given person does not exist");
            }
            person.PersonName = personUpdateRequest.PersonName;
            person.DateOfBirth = personUpdateRequest.DateOfBirth;
            person.Gender = personUpdateRequest.Gender.ToString();
            person.CountryID = personUpdateRequest.CountryID;
            person.Email = personUpdateRequest.Email;
            person.Address = personUpdateRequest.Address;
            person.RecievesNewsLetters = personUpdateRequest.RecievesNewsLetters;
            
            return ConvertPersonToPersonResponse(person);
        }

        public bool DeletePerson(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }
            var person = _persons.FirstOrDefault(p => p.PersonID == personID);
            _persons.RemoveAll(p => p.PersonID == personID);
            return (person != null) ? true : false;
        }
    }
}
