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
        private readonly ApplicationDbContext _db;
        private readonly ICountriesService _countriesService;

        public PersonsService(ApplicationDbContext db , ICountriesService countriesService)
        {
            _db = db;
            _countriesService = countriesService;
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
            _db.Persons.Add(createdPerson);
            _db.SaveChanges();
            return ConvertPersonToPersonResponse(createdPerson);
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _db.Persons.ToList().Select(p => ConvertPersonToPersonResponse(p)).ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }
            var person = _db.Persons.Where(p => p.PersonID == personID)?.FirstOrDefault();
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
            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            {
                return allPersons;
            }

            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons.Where(p=> p.PersonName != null && p.PersonName.Contains(searchString , StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons.Where(p => p.DateOfBirth != null && p.DateOfBirth.Value.ToString("dd/MM/yyyy").Contains(searchString)).ToList();
                    break;
                
                case nameof(PersonResponse.Gender):
                    matchingPersons = allPersons.Where(p => p.Gender != null && p.Gender.Equals(searchString , StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                
                case nameof(PersonResponse.Country):
                    matchingPersons = allPersons.Where(p=> p.Country != null && p.Country.Contains(searchString , StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                
                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons.Where(p => p.Email != null && p.Email.Contains(searchString , StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                
                case nameof(PersonResponse.Address):
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
            
            var person = _db.Persons.FirstOrDefault(p=>p.PersonID == personUpdateRequest.PersonID);
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
            _db.SaveChanges();            
            return ConvertPersonToPersonResponse(person);
        }

        public bool DeletePerson(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }
            var person = _db.Persons.FirstOrDefault(p => p.PersonID == personID);
            _db.Persons.Remove(_db.Persons.First(p=>p.PersonID == personID));
            _db.SaveChanges();
            return (person != null) ? true : false;
        }   
    }
}
