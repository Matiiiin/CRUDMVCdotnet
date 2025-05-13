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
using ServiceContracts.Persons;

namespace Services.Persons
{
    public class PersonsGetterService(IPersonsRepository _personsRepository , IDiagnosticContext _diagnosticContext) : IPersonsGetterService
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
    }
}
