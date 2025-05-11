using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using RepositoryContracts;
using Serilog;
using ServiceContracts.Enums;

namespace Services
{
    public class PersonsService(IPersonsRepository _personsRepository , IDiagnosticContext _diagnosticContext) : IPersonsService
    {
        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(PersonAddRequest));
            }


            ModelValidation.Validate(personAddRequest);

            // _db.sp_AddPerson(createdPerson);
            return (await _personsRepository.AddPerson(personAddRequest.ToPerson())).ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            // return _db.Persons.sp_GetAllPersons().Select(p => ConvertPersonToPersonResponse(p)).ToList();
            return (await _personsRepository.GetAllPersons()).Select(p=>p.ToPersonResponse()).ToList();
        }

        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }
            // var person = _db.Persons.sp_GetPersonByID(personID);
            var person = await _personsRepository.GetPersonByPersonID(personID);
            return person?.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string? searchBy, string? searchString)
        {
            List<PersonResponse> matchingPersons = new(){};
            if (searchString.IsNullOrEmpty())
            {
                return (await _personsRepository.GetAllPersons()).Select(p => p.ToPersonResponse()).ToList();
            }
            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = (await _personsRepository.GetFilteredPersons(p=> p.PersonName.Contains(searchString))).Select(p=>p.ToPersonResponse()).ToList();
                    break;

                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = (await _personsRepository.GetFilteredPersons(temp => temp.DateOfBirth.Value.ToString().Contains(searchString))).Select(p => p.ToPersonResponse()).ToList();
                    break;

                
                case nameof(PersonResponse.Gender):
                    matchingPersons = (await _personsRepository.GetFilteredPersons(p => p.Gender != null && p.Gender.Equals(searchString))).Select(p=>p.ToPersonResponse()).ToList();
                    break;
                
                case nameof(PersonResponse.Country):
                    matchingPersons = (await _personsRepository.GetFilteredPersons(p=> p.Country != null && p.Country.CountryName.Contains(searchString))).Select(p=>p.ToPersonResponse()).ToList();
                    break;
                
                case nameof(PersonResponse.Email):
                    matchingPersons = (await _personsRepository.GetFilteredPersons(p => p.Email != null && p.Email.Contains(searchString))).Select(p=>p.ToPersonResponse()).ToList();
                    break;
                
                case nameof(PersonResponse.Address):
                    matchingPersons = (await _personsRepository.GetFilteredPersons(p => p.Address != null && p.Address.Contains(searchString))).Select(p=>p.ToPersonResponse()).ToList();
                    break;
                default:
                    matchingPersons = (await _personsRepository.GetAllPersons()).Select(p=>p.ToPersonResponse()).ToList();
                    break;
            }
            _diagnosticContext.Set("Persons" , matchingPersons);
            return matchingPersons;

        }
        
        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> persons, string sortBy, SortOrderOptions sortOrder)
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

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(personUpdateRequest));
            }
            ModelValidation.Validate(personUpdateRequest);
            
            // var person = _db.Persons.sp_GetPersonByID(personUpdateRequest.PersonID);
            if (await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID) == null)
            {
                throw new ArgumentException("Given person does not exist");
            }

            // _db.sp_UpdatePerson(personUpdateRequest.ToPerson());
            var x = await _personsRepository.UpdatePerson(personUpdateRequest.ToPerson());
            return (x).ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }
            var person = await _personsRepository.GetPersonByPersonID(personID);
            if (person == null) return false;
            await _personsRepository.DeletePerson(personID);
            return true;
        }   
    }
}
